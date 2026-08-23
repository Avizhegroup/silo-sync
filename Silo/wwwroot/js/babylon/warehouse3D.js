// Babylon.js 3D Warehouse Visualization
var Warehouse3D = (function () {
    let engine = null;
    let scene = null;
    let camera = null;
    let canvas = null;
    let warehouses = [];
    let zones = [];
    let warehouseBoxes = new Map();
    let zoneBoxes = new Map();
    let isShowingWarehouseList = true;
    let selectedWarehouse = null;
    let dotNetHelper = null;
    let lastLoadedWarehouseCount = 0;
    let lastLoadedZoneCount = 0;
    let isDragging = false;

    // Corridor state
    let corridorMeshes = new Map();       // corridorId (int) -> { tube, mat }
    let corridorDrawMode = false;
    let corridorRelocateMode = false;     // true when user is repositioning a corridor
    let relocatingCorridorId = null;      // id of corridor being relocated
    let corridorDrawWidth = 1.0;          // editable width
    let corridorStartPoint = null;        // BABYLON.Vector3 | null
    let corridorPreviewTube = null;       // live preview mesh
    let corridorStartMarker = null;       // sphere marker at start point
    let corridorMouseMoveHandler = null;  // pointer-move observer for preview

    // Initialize the 3D scene
    function initialize(canvasId, dotNetRef) {
        dotNetHelper = dotNetRef;

        canvas = document.getElementById(canvasId);
        if (!canvas) {
            console.error("Canvas not found:", canvasId);
            return false;
        }

        try {
            engine = new BABYLON.Engine(canvas, true, {
                preserveDrawingBuffer: true,
                stencil: true
            });

            scene = new BABYLON.Scene(engine);
            scene.clearColor = new BABYLON.Color4(0.95, 0.95, 0.97, 1);

            // Create camera
            camera = new BABYLON.ArcRotateCamera(
                "camera",
                Math.PI / 4,
                Math.PI / 3,
                30,
                BABYLON.Vector3.Zero(),
                scene
            );
            camera.attachControl(canvas, true);
            camera.lowerRadiusLimit = 10;
            camera.upperRadiusLimit = 100;
            camera.wheelPrecision = 50;

            // Create lights
            const light1 = new BABYLON.HemisphericLight(
                "light1",
                new BABYLON.Vector3(1, 1, 0),
                scene
            );
            light1.intensity = 0.7;

            const light2 = new BABYLON.DirectionalLight(
                "light2",
                new BABYLON.Vector3(-1, -2, -1),
                scene
            );
            light2.intensity = 0.5;

            // Add ground
            const ground = BABYLON.MeshBuilder.CreateGround(
                "ground",
                { width: 100, height: 100 },
                scene
            );
            const groundMaterial = new BABYLON.StandardMaterial("groundMat", scene);
            groundMaterial.diffuseColor = new BABYLON.Color3(0.9, 0.9, 0.9);
            groundMaterial.specularColor = new BABYLON.Color3(0.1, 0.1, 0.1);
            ground.material = groundMaterial;
            ground.position.y = -0.5;

            // Grid
            const gridMaterial = new BABYLON.GridMaterial("gridMat", scene);
            gridMaterial.majorUnitFrequency = 5;
            gridMaterial.minorUnitVisibility = 0.3;
            gridMaterial.gridRatio = 1;
            gridMaterial.backFaceCulling = false;
            gridMaterial.mainColor = new BABYLON.Color3(0.8, 0.8, 0.8);
            gridMaterial.lineColor = new BABYLON.Color3(0.7, 0.7, 0.7);
            gridMaterial.opacity = 0.8;
            ground.material = gridMaterial;

            // Enable picking — only treat as click when the pointer hasn't moved (not a drag)
            scene.onPointerObservable.add((pointerInfo) => {
                if (pointerInfo.type === BABYLON.PointerEventTypes.POINTERDOWN) {
                    isDragging = false;
                }
                if (pointerInfo.type === BABYLON.PointerEventTypes.POINTERMOVE) {
                    isDragging = true;
                }
                if (pointerInfo.type === BABYLON.PointerEventTypes.POINTERUP) {
                    if (!isDragging) {
                        if (corridorDrawMode) {
                            // In corridor-draw mode every left-click picks a ground point
                            const pt = pickGroundPoint(pointerInfo);
                            if (pt) handleCorridorGroundClick(pt);
                        } else if (corridorRelocateMode) {
                            // In relocate mode left-click on ground picks the new endpoint
                            if (pointerInfo.pickInfo.hit && pointerInfo.pickInfo.pickedMesh) {
                                handleCorridorRelocateClick(pointerInfo);
                            }
                        } else if (pointerInfo.pickInfo.hit && pointerInfo.pickInfo.pickedMesh) {
                            handleMeshClick(pointerInfo.pickInfo.pickedMesh);
                        }
                    }
                    isDragging = false;
                }
            });

            // Right-click on canvas to show context menu for corridors
            canvas.addEventListener('contextmenu', (evt) => {
                evt.preventDefault();
                if (corridorDrawMode || corridorRelocateMode) return;
                const pickResult = scene.pick(evt.offsetX, evt.offsetY);
                if (pickResult.hit && pickResult.pickedMesh && pickResult.pickedMesh.metadata && pickResult.pickedMesh.metadata.type === 'corridor') {
                    const corridorId = pickResult.pickedMesh.metadata.corridorId;
                    if (dotNetHelper) {
                        dotNetHelper.invokeMethodAsync('OnCorridorRightClicked', corridorId, evt.clientX, evt.clientY)
                            .catch(err => console.error('OnCorridorRightClicked error:', err));
                    }
                }
            });

            // Render loop
            engine.runRenderLoop(() => {
                scene.render();
            });

            // Resize handler
            window.addEventListener("resize", () => {
                engine.resize();
            });

            return true;
        } catch (error) {
            console.error("Error initializing Babylon.js:", error);
            return false;
        }
    }

    // Load and display warehouses as 3D boxes
    function loadWarehouses(warehouseData) {
        if (!warehouseData || warehouseData.length === 0) {
            return;
        }

        // Prevent redundant reloads
        if (warehouseData.length === lastLoadedWarehouseCount && isShowingWarehouseList) {
            return;
        }

        lastLoadedWarehouseCount = warehouseData.length;
        warehouses = Array.isArray(warehouseData) ? warehouseData : [warehouseData];
        isShowingWarehouseList = true;
        selectedWarehouse = null;

        // Clear existing meshes
        clearScene();

        const spacing = 12;
        const cols = Math.ceil(Math.sqrt(warehouses.length));
        const startX = -(cols * spacing) / 2;
        const startZ = -(cols * spacing) / 2;

        warehouses.forEach((warehouse, index) => {
            const row = Math.floor(index / cols);
            const col = index % cols;

            const box = createWarehouseBox(warehouse, startX + col * spacing, startZ + row * spacing);
            warehouseBoxes.set(warehouse.destinationCode, { mesh: box, data: warehouse });
        });

        // Adjust camera
        camera.setTarget(BABYLON.Vector3.Zero());
        camera.radius = 30;

        // Draw corridors after warehouses are placed (called from C# after loadWarehouses)
    }

    // Create a 3D box for warehouse
    function createWarehouseBox(warehouse, x, z) {
        const height = 5;
        const box = BABYLON.MeshBuilder.CreateBox(
            `warehouse_${warehouse.destinationCode}`,
            { width: 8, height: height, depth: 8 },
            scene
        );

        box.position = new BABYLON.Vector3(x, height / 2, z);

        // Restore saved position if available (format: "x,y,z")
        if (warehouse.coordinates) {
            const parts = warehouse.coordinates.split(',').map(Number);
            if (parts.length === 3 && parts.every(n => !isNaN(n))) {
                box.position = new BABYLON.Vector3(parts[0], parts[1], parts[2]);
            }
        }

        box.metadata = { type: 'warehouse', data: warehouse };

        // Material based on operational type
        const material = new BABYLON.StandardMaterial(`mat_${warehouse.destinationCode}`, scene);
        material.diffuseColor = getWarehouseColor(warehouse.operationalType);
        material.specularColor = new BABYLON.Color3(0.2, 0.2, 0.2);
        box.material = material;

        // Add hover effect
        box.actionManager = new BABYLON.ActionManager(scene);
        box.actionManager.registerAction(
            new BABYLON.ExecuteCodeAction(
                BABYLON.ActionManager.OnPointerOverTrigger,
                () => {
                    material.emissiveColor = new BABYLON.Color3(0.2, 0.2, 0.2);
                    showWarehouseTooltip(warehouse);
                }
            )
        );
        box.actionManager.registerAction(
            new BABYLON.ExecuteCodeAction(
                BABYLON.ActionManager.OnPointerOutTrigger,
                () => {
                    material.emissiveColor = new BABYLON.Color3(0, 0, 0);
                    hideWarehouseTooltip();
                }
            )
        );

        const labelPlane = BABYLON.MeshBuilder.CreatePlane("label", { size: 5 }, scene);
        labelPlane.position = box.position.add(new BABYLON.Vector3(0, height / 2 + 2, 0));
        labelPlane.billboardMode = BABYLON.Mesh.BILLBOARDMODE_ALL;
        labelPlane.metadata = { isLabel: true };
        labelPlane.parent = box;
        labelPlane.position = new BABYLON.Vector3(0, height / 2 + 2, 0);

        const labelTexture = BABYLON.GUI.AdvancedDynamicTexture.CreateForMesh(labelPlane);
        const labelText = new BABYLON.GUI.TextBlock();
        labelText.text = warehouse.destinationTitle;
        labelText.color = "white";
        labelText.fontSize = 72;

        // Drag behaviour — constrained to XZ plane
        const dragBehavior = new BABYLON.PointerDragBehavior({ dragPlaneNormal: new BABYLON.Vector3(0, 1, 0) });
        dragBehavior.useObjectOrientationForDragging = false;
        dragBehavior.onDragStartObservable.add(() => { isDragging = true; });
        dragBehavior.onDragEndObservable.add(() => {
            if (dotNetHelper) {
                dotNetHelper.invokeMethodAsync('OnCubePositionChanged',
                    warehouse.destinationCode, 'warehouse',
                    box.position.x, box.position.y, box.position.z)
                    .catch(err => console.error('OnCubePositionChanged error:', err));
            }
        });
        box.addBehavior(dragBehavior);
        labelText.fontWeight = "bold";
        labelText.outlineWidth = 8;
        labelText.outlineColor = "black";
        labelText.textWrapping = true;
        labelTexture.addControl(labelText);

        return box;
    }

    // Load and display zones as 3D boxes
    function loadZones(zoneData, warehouseCode) {
        if (!zoneData || zoneData.length === 0) {
            return;
        }

        // Prevent redundant reloads
        if (zoneData.length === lastLoadedZoneCount && !isShowingWarehouseList && selectedWarehouse === warehouseCode) {
            return;
        }

        lastLoadedZoneCount = zoneData.length;
        zones = zoneData;
        isShowingWarehouseList = false;
        selectedWarehouse = warehouseCode;

        clearScene();

        if (zones.length === 0) {
            return;
        }

        const spacing = 5;
        const cols = Math.ceil(Math.sqrt(zones.length));
        const rows = Math.ceil(zones.length / cols);
        const startX = -(cols * spacing) / 2;
        const startZ = -(rows * spacing) / 2;

        zones.forEach((zone, index) => {
            const row = Math.floor(index / cols);
            const col = index % cols;
            const posX = startX + col * spacing;
            const posZ = startZ + row * spacing;

            const box = createZoneBox(zone, posX, posZ);
            zoneBoxes.set(zone.zoneCode, { mesh: box, data: zone });
        });

        // Adjust camera to fit all zones
        camera.setTarget(BABYLON.Vector3.Zero());
        camera.radius = Math.max(20, Math.sqrt(zones.length) * 3);

        // Ensure scene renders
        scene.executeWhenReady(() => {
            engine.resize();
            scene.render();
        });
    }

    // Create a 3D box for zone with capacity-based color
    function createZoneBox(zone, x, z) {
        const percent = zone.capacity > 0 ? (zone.occupiedCapacity / zone.capacity) * 100 : 0;
        const minHeight = 2;
        const maxHeight = 8;
        const height = minHeight + (Math.min(percent, 100) / 100) * (maxHeight - minHeight);

        const box = BABYLON.MeshBuilder.CreateBox(
            `zone_${zone.zoneCode}`,
            { width: 3.5, height: height, depth: 3.5 },
            scene
        );

        box.position = new BABYLON.Vector3(x, height / 2, z);

        // Restore saved position if available (format: "x,y,z")
        if (zone.coordinates) {
            const parts = zone.coordinates.split(',').map(Number);
            if (parts.length === 3 && parts.every(n => !isNaN(n))) {
                box.position = new BABYLON.Vector3(parts[0], parts[1], parts[2]);
            }
        }

        box.metadata = { type: 'zone', data: zone };

        // Material based on capacity
        const material = new BABYLON.StandardMaterial(`mat_${zone.zoneCode}`, scene);
        const colorInfo = getZoneColorFromCapacity(zone);
        material.diffuseColor = colorInfo.color;
        material.specularColor = new BABYLON.Color3(0.3, 0.3, 0.3);
        material.alpha = 0.95;
        box.material = material;

        // Add hover and click effects
        box.actionManager = new BABYLON.ActionManager(scene);
        box.actionManager.registerAction(
            new BABYLON.ExecuteCodeAction(
                BABYLON.ActionManager.OnPointerOverTrigger,
                () => {
                    material.emissiveColor = new BABYLON.Color3(0.3, 0.3, 0.3);
                    material.alpha = 1.0;
                    showZoneTooltip(zone);
                }
            )
        );
        box.actionManager.registerAction(
            new BABYLON.ExecuteCodeAction(
                BABYLON.ActionManager.OnPointerOutTrigger,
                () => {
                    material.emissiveColor = new BABYLON.Color3(0, 0, 0);
                    material.alpha = 0.95;
                    hideZoneTooltip();
                }
            )
        );

        // Determine what text to show - prefer title, fallback to code if title is missing/same as code
        let labelText = zone.title;
        if (!labelText || labelText === zone.zoneCode || labelText.trim() === '') {
            labelText = zone.zoneCode;
        }

        // Add zone label at the top — parented so it follows the box when dragged
        const labelHeight = height / 2 + 1.2;
        const labelPlane = BABYLON.MeshBuilder.CreatePlane("label", { size: 3.5 }, scene);
        labelPlane.billboardMode = BABYLON.Mesh.BILLBOARDMODE_ALL;
        labelPlane.metadata = { isLabel: true };
        labelPlane.parent = box;
        labelPlane.position = new BABYLON.Vector3(0, labelHeight, 0);

        const labelTexture = BABYLON.GUI.AdvancedDynamicTexture.CreateForMesh(labelPlane);
        const labelTextBlock = new BABYLON.GUI.TextBlock();
        labelTextBlock.text = labelText;
        labelTextBlock.color = "white";
        labelTextBlock.fontSize = 64;
        labelTextBlock.fontWeight = "bold";
        labelTextBlock.outlineWidth = 6;
        labelTextBlock.outlineColor = "black";
        labelTextBlock.textWrapping = true;
        labelTexture.addControl(labelTextBlock);

        // Drag behaviour — constrained to XZ plane
        const dragBehavior = new BABYLON.PointerDragBehavior({ dragPlaneNormal: new BABYLON.Vector3(0, 1, 0) });
        dragBehavior.useObjectOrientationForDragging = false;
        dragBehavior.onDragStartObservable.add(() => { isDragging = true; });
        dragBehavior.onDragEndObservable.add(() => {
            if (dotNetHelper) {
                dotNetHelper.invokeMethodAsync('OnCubePositionChanged',
                    zone.zoneCode, 'zone',
                    box.position.x, box.position.y, box.position.z)
                    .catch(err => console.error('OnCubePositionChanged error:', err));
            }
        });
        box.addBehavior(dragBehavior);

        return box;
    }

    // Create text label using GUI
    function createTextLabel(text, position, fontSize = 64, highContrast = false) {
        const plane = BABYLON.MeshBuilder.CreatePlane("label", { size: 2.5 }, scene);
        plane.position = position;
        plane.billboardMode = BABYLON.Mesh.BILLBOARDMODE_ALL;

        const advancedTexture = BABYLON.GUI.AdvancedDynamicTexture.CreateForMesh(plane);
        const textBlock = new BABYLON.GUI.TextBlock();
        textBlock.text = text;
        textBlock.color = highContrast ? "#ffffff" : "#1a252f";
        textBlock.fontSize = fontSize;
        textBlock.fontWeight = "bold";
        textBlock.outlineWidth = highContrast ? 6 : 4;
        textBlock.outlineColor = highContrast ? "#1a252f" : "white";
        advancedTexture.addControl(textBlock);

        return plane;
    }

    // Get color based on warehouse operational type
    function getWarehouseColor(operationalType) {
        switch (operationalType) {
            case 1: // Product
                return new BABYLON.Color3(0.2, 0.6, 0.9); // Blue
            case 2: // Loading
                return new BABYLON.Color3(0.9, 0.6, 0.2); // Orange
            case 3: // Waste
                return new BABYLON.Color3(0.7, 0.3, 0.3); // Red
            case 4: // Quarantine
                return new BABYLON.Color3(0.9, 0.9, 0.3); // Yellow
            case 5: // Sales
                return new BABYLON.Color3(0.3, 0.8, 0.3); // Green
            case 6: // Material
                return new BABYLON.Color3(0.6, 0.4, 0.2); // Brown
            default:
                return new BABYLON.Color3(0.7, 0.7, 0.7); // Gray
        }
    }

    // Get color based on zone capacity
    function getZoneColorFromCapacity(zone) {
        if (zone.capacity <= 0 || zone.occupiedCapacity <= 0) {
            return { color: new BABYLON.Color3(0.93, 0.94, 0.95), text: "#2c3e50" }; // Light gray
        }

        const percent = (zone.occupiedCapacity / zone.capacity) * 100;

        if (percent <= 25) {
            return { color: new BABYLON.Color3(0.93, 0.94, 0.95), text: "#2c3e50" }; // Light gray
        } else if (percent <= 50) {
            return { color: new BABYLON.Color3(0.95, 0.77, 0.06), text: "#2c3e50" }; // Yellow
        } else if (percent <= 75) {
            return { color: new BABYLON.Color3(0.90, 0.49, 0.13), text: "#ffffff" }; // Orange
        } else {
            return { color: new BABYLON.Color3(0.91, 0.30, 0.24), text: "#ffffff" }; // Red
        }
    }

    // Show tooltip for zone
    function showZoneTooltip(zone) {
        const tooltip = document.getElementById('zone-tooltip-3d');
        if (tooltip) {
            const percent = zone.capacity > 0 ? Math.min((zone.occupiedCapacity / zone.capacity) * 100, 100) : 0;
            tooltip.innerHTML = `
                <strong>${zone.title}</strong><br/>
                کد زون: ${zone.zoneCode}<br/>
                ظرفیت: ${zone.capacity}<br/>
                اشغال شده: ${zone.occupiedCapacity}<br/>
                درصد: ${Math.floor(percent)}%
            `;
            tooltip.style.display = 'block';
        }
    }

    // Hide tooltip
    function hideZoneTooltip() {
        const tooltip = document.getElementById('zone-tooltip-3d');
        if (tooltip) {
            tooltip.style.display = 'none';
        }
    }

    // Show tooltip for warehouse
    function showWarehouseTooltip(warehouse) {
        const tooltip = document.getElementById('zone-tooltip-3d');
        if (tooltip) {
            const operationalTypeNames = {
                0: "تعریف نشده",
                1: "محصول",
                2: "بارگیری",
                3: "ضایعات",
                4: "قرنطینه",
                5: "فروش",
                6: "مواد اولیه"
            };

            const typeName = operationalTypeNames[warehouse.operationalType] || "نامشخص";
            const statusText = warehouse.isActive ? "فعال" : "غیرفعال";

            tooltip.innerHTML = `
                <strong>${warehouse.destinationTitle}</strong><br/>
                کد انبار: ${warehouse.destinationCode}<br/>
                نوع عملیاتی: ${typeName}<br/>
                وضعیت: ${statusText}
            `;
            tooltip.style.display = 'block';
        }
    }

    // Hide warehouse tooltip
    function hideWarehouseTooltip() {
        const tooltip = document.getElementById('zone-tooltip-3d');
        if (tooltip) {
            tooltip.style.display = 'none';
        }
    }

    // Handle mesh click (only called when NOT in corridorDrawMode)
    function handleMeshClick(mesh) {
        if (!mesh.metadata) return;

        if (mesh.metadata.type === 'warehouse') {
            if (dotNetHelper) {
                dotNetHelper.invokeMethodAsync('OnWarehouseSelected3D', mesh.metadata.data.destinationCode)
                    .catch(error => console.error("Error calling OnWarehouseSelected3D:", error));
            }
        } else if (mesh.metadata.type === 'corridor') {
            if (corridorRelocateMode && relocatingCorridorId === mesh.metadata.corridorId) {
                // In relocate mode a left-click on the same corridor does nothing special
            }
            // Left-click on corridor in relocate mode selects it as the second point
        }
    }

    // Clear all 3D objects from scene
    function clearScene() {
        warehouseBoxes.forEach(({ mesh }) => {
            if (mesh && !mesh.isDisposed()) {
                mesh.dispose();
            }
        });
        zoneBoxes.forEach(({ mesh }) => {
            if (mesh && !mesh.isDisposed()) {
                mesh.dispose();
            }
        });

        // Clear corridor meshes
        corridorMeshes.forEach(({ tube }) => {
            if (tube && !tube.isDisposed()) tube.dispose();
        });
        corridorMeshes.clear();

        // Clear labels and other meshes (excluding ground and essential elements)
        const meshesToRemove = [];
        scene.meshes.forEach(mesh => {
            if (mesh.name.startsWith('label') || 
                mesh.name.startsWith('zone_') || 
                mesh.name.startsWith('warehouse_') ||
                mesh.name.startsWith('corridor_') ||
                mesh.metadata?.isLabel) {
                meshesToRemove.push(mesh);
            }
        });

        meshesToRemove.forEach(mesh => {
            if (mesh && !mesh.isDisposed()) {
                mesh.dispose();
            }
        });

        warehouseBoxes.clear();
        zoneBoxes.clear();
    }

    // Go back to warehouse list
    function backToWarehouses() {
        if (!isShowingWarehouseList) {
            return true; // Signal to reload warehouses
        }
        return false;
    }

    // Dispose the scene
    function dispose() {
        if (engine) {
            engine.dispose();
        }
        warehouses = [];
        zones = [];
        warehouseBoxes.clear();
        zoneBoxes.clear();
        corridorMeshes.clear();
        corridorDrawMode = false;
        corridorStartPoint = null;
        corridorDrawWidth = 1.0;
        if (corridorPreviewTube && !corridorPreviewTube.isDisposed()) corridorPreviewTube.dispose();
        corridorPreviewTube = null;
        if (corridorStartMarker && !corridorStartMarker.isDisposed()) corridorStartMarker.dispose();
        corridorStartMarker = null;
        dotNetHelper = null;
        lastLoadedWarehouseCount = 0;
        lastLoadedZoneCount = 0;
    }

    // Return current positions of all visible cubes
    function getPositions() {
        const result = [];
        warehouseBoxes.forEach(({ mesh, data }) => {
            if (mesh && !mesh.isDisposed()) {
                result.push({
                    code: data.destinationCode,
                    type: 'warehouse',
                    x: mesh.position.x,
                    y: mesh.position.y,
                    z: mesh.position.z
                });
            }
        });
        zoneBoxes.forEach(({ mesh, data }) => {
            if (mesh && !mesh.isDisposed()) {
                result.push({
                    code: data.zoneCode,
                    type: 'zone',
                    x: mesh.position.x,
                    y: mesh.position.y,
                    z: mesh.position.z
                });
            }
        });
        return result;
    }

    // Zoom by adjusting camera radius (positive = zoom out, negative = zoom in)
    function zoom(delta) {
        if (!camera) return;
        const newRadius = camera.radius + delta;
        camera.radius = Math.max(camera.lowerRadiusLimit, Math.min(camera.upperRadiusLimit, newRadius));
    }

    // ─── Corridor drawing ───────────────────────────────────────────────────────

    // Pick a point on the ground plane from a pointer event
    function pickGroundPoint(pointerInfo) {
        const pickResult = scene.pick(
            scene.pointerX,
            scene.pointerY,
            mesh => mesh.name === 'ground'
        );
        if (pickResult.hit) return pickResult.pickedPoint;
        // Fallback: ray-plane intersection at Y=0
        const ray = scene.createPickingRay(scene.pointerX, scene.pointerY, BABYLON.Matrix.Identity(), camera);
        const t = -ray.origin.y / ray.direction.y;
        if (t > 0) return ray.origin.add(ray.direction.scale(t));
        return null;
    }

    // Build a corridor tube mesh from two Vector3 ground points and a radius
    function buildCorridorTube(name, from, to, width, existing) {
        const radius = Math.max(0.1, width / 2);
        const path = [
            new BABYLON.Vector3(from.x, 0.15, from.z),
            new BABYLON.Vector3((from.x + to.x) / 2, 0.3, (from.z + to.z) / 2),
            new BABYLON.Vector3(to.x, 0.15, to.z)
        ];
        if (existing && !existing.isDisposed()) existing.dispose();
        return BABYLON.MeshBuilder.CreateTube(
            name,
            { path, radius, tessellation: 14, updatable: false },
            scene
        );
    }

    // Update the live preview tube while moving the mouse
    function updateCorridorPreview(toPoint) {
        if (!corridorStartPoint || !toPoint) return;
        corridorPreviewTube = buildCorridorTube('corridorPreview', corridorStartPoint, toPoint, corridorDrawWidth, corridorPreviewTube);
        if (!corridorPreviewTube.material) {
            const mat = new BABYLON.StandardMaterial('corridorPreviewMat', scene);
            mat.diffuseColor = new BABYLON.Color3(0.9, 0.8, 0.1);
            mat.emissiveColor = new BABYLON.Color3(0.3, 0.25, 0.0);
            mat.alpha = 0.6;
            corridorPreviewTube.material = mat;
        }
        corridorPreviewTube.isPickable = false;
    }

    // Finalize and render a saved corridor from coordinates
    function renderCorridorFromData(c) {
        const from = new BABYLON.Vector3(c.x1, 0.15, c.z1);
        const to   = new BABYLON.Vector3(c.x2, 0.15, c.z2);
        const tube = buildCorridorTube(`corridor_${c.id}`, from, to, c.width);

        const mat = new BABYLON.StandardMaterial(`corridorMat_${c.id}`, scene);
        mat.diffuseColor = new BABYLON.Color3(0.2, 0.75, 0.45);
        mat.emissiveColor = new BABYLON.Color3(0.03, 0.15, 0.08);
        mat.alpha = 0.88;
        tube.material = mat;
        tube.metadata = { type: 'corridor', corridorId: c.id, label: c.label, width: c.width };

        tube.actionManager = new BABYLON.ActionManager(scene);
        tube.actionManager.registerAction(new BABYLON.ExecuteCodeAction(
            BABYLON.ActionManager.OnPointerOverTrigger,
            () => {
                mat.emissiveColor = new BABYLON.Color3(0.5, 0.1, 0.1);
                const tooltip = document.getElementById('zone-tooltip-3d');
                if (tooltip) {
                    const lbl = c.label ? `<strong>${c.label}</strong><br/>` : '';
                    tooltip.innerHTML = `${lbl}عرض: ${c.width.toFixed(1)} متر<br/><small>کلیک برای حذف</small>`;
                    tooltip.style.display = 'block';
                }
            }
        ));
        tube.actionManager.registerAction(new BABYLON.ExecuteCodeAction(
            BABYLON.ActionManager.OnPointerOutTrigger,
            () => {
                mat.emissiveColor = new BABYLON.Color3(0.03, 0.15, 0.08);
                const tooltip = document.getElementById('zone-tooltip-3d');
                if (tooltip) tooltip.style.display = 'none';
            }
        ));

        corridorMeshes.set(c.id, { tube, mat });
    }

    // Load / refresh all corridors for current context
    function loadCorridors(corridorData) {
        corridorMeshes.forEach(({ tube }) => { if (tube && !tube.isDisposed()) tube.dispose(); });
        corridorMeshes.clear();
        if (!corridorData || corridorData.length === 0) return;
        corridorData.forEach(c => renderCorridorFromData(c));
    }

    // Remove a single corridor mesh by id
    function removeCorridorMesh(corridorId) {
        const entry = corridorMeshes.get(corridorId);
        if (entry?.tube && !entry.tube.isDisposed()) entry.tube.dispose();
        corridorMeshes.delete(corridorId);
    }

    // Enable / disable corridor-draw mode
    function setCorridorDrawMode(enabled, width) {
        corridorDrawMode = enabled;
        if (width !== undefined) corridorDrawWidth = width;

        // Cancel any in-progress drawing
        if (!enabled) {
            corridorStartPoint = null;
            if (corridorPreviewTube && !corridorPreviewTube.isDisposed()) corridorPreviewTube.dispose();
            corridorPreviewTube = null;
            if (corridorStartMarker && !corridorStartMarker.isDisposed()) corridorStartMarker.dispose();
            corridorStartMarker = null;
            if (corridorMouseMoveHandler) {
                scene.onPointerObservable.remove(corridorMouseMoveHandler);
                corridorMouseMoveHandler = null;
            }
        }

        // Disable / re-enable camera drag so it doesn't interfere with draw clicks
        if (camera) {
            camera.inputs.attached.pointers && (camera.inputs.attached.pointers.buttons = enabled ? [1, 2] : [0, 1, 2]);
        }

        const tooltip = document.getElementById('zone-tooltip-3d');
        if (tooltip) {
            if (enabled) {
                tooltip.innerHTML = 'حالت ترسیم راهرو: روی زمین کلیک کنید (نقطه شروع)';
                tooltip.style.display = 'block';
            } else {
                tooltip.style.display = 'none';
            }
        }

        // Register / remove pointer-move handler for live preview
        if (enabled && !corridorMouseMoveHandler) {
            corridorMouseMoveHandler = scene.onPointerObservable.add((pointerInfo) => {
                if (pointerInfo.type === BABYLON.PointerEventTypes.POINTERMOVE && corridorStartPoint) {
                    const pt = pickGroundPoint(pointerInfo);
                    if (pt) updateCorridorPreview(pt);
                }
            });
        }
    }

    // Update width of in-progress preview (called when slider changes)
    function setCorridorWidth(width) {
        corridorDrawWidth = Math.max(0.2, width);
        // Rebuild preview immediately if currently drawing
        if (corridorPreviewTube && corridorStartPoint) {
            const pt = pickGroundPoint(null); // best-effort; preview refreshes on next POINTERMOVE anyway
        }
    }

    // Called from handleMeshClick / pointer-up when in draw mode and user clicks the ground
    function handleCorridorGroundClick(point) {
        const tooltip = document.getElementById('zone-tooltip-3d');

        if (!corridorStartPoint) {
            // First click — place start marker
            corridorStartPoint = point.clone();

            if (corridorStartMarker && !corridorStartMarker.isDisposed()) corridorStartMarker.dispose();
            corridorStartMarker = BABYLON.MeshBuilder.CreateSphere('corridorStart', { diameter: corridorDrawWidth * 0.8, segments: 8 }, scene);
            corridorStartMarker.position = new BABYLON.Vector3(corridorStartPoint.x, 0.3, corridorStartPoint.z);
            const mMat = new BABYLON.StandardMaterial('corridorStartMat', scene);
            mMat.diffuseColor = new BABYLON.Color3(0.9, 0.8, 0.1);
            mMat.emissiveColor = new BABYLON.Color3(0.4, 0.3, 0.0);
            corridorStartMarker.material = mMat;
            corridorStartMarker.isPickable = false;

            if (tooltip) { tooltip.innerHTML = 'نقطه شروع ثبت شد — نقطه پایان را انتخاب کنید'; tooltip.style.display = 'block'; }
        } else {
            // Second click — finalize
            const endPoint = point.clone();

            // Clean up preview
            if (corridorPreviewTube && !corridorPreviewTube.isDisposed()) corridorPreviewTube.dispose();
            corridorPreviewTube = null;
            if (corridorStartMarker && !corridorStartMarker.isDisposed()) corridorStartMarker.dispose();
            corridorStartMarker = null;

            const from = corridorStartPoint;
            corridorStartPoint = null;

            if (tooltip) { tooltip.innerHTML = 'در حال ذخیره راهرو ...'; tooltip.style.display = 'block'; }

            if (dotNetHelper) {
                dotNetHelper.invokeMethodAsync(
                    'OnCorridorDrawn',
                    from.x, from.z,
                    endPoint.x, endPoint.z,
                    corridorDrawWidth
                ).catch(err => console.error('OnCorridorDrawn error:', err));
            }
        }
    }

    // Start relocating an existing corridor — the user will click two new ground points
    function startCorridorRelocate(corridorId) {
        relocatingCorridorId = corridorId;
        corridorRelocateMode = true;
        corridorStartPoint = null;

        // Disable dragging on all warehouse/zone boxes
        warehouseBoxes.forEach(({ mesh }) => {
            if (mesh) mesh.behaviors.forEach(b => { if (b instanceof BABYLON.PointerDragBehavior) b.enabled = false; });
        });
        zoneBoxes.forEach(({ mesh }) => {
            if (mesh) mesh.behaviors.forEach(b => { if (b instanceof BABYLON.PointerDragBehavior) b.enabled = false; });
        });

        // Disable camera drag so it doesn't interfere
        if (camera) {
            camera.inputs.attached.pointers && (camera.inputs.attached.pointers.buttons = [1, 2]);
        }

        const tooltip = document.getElementById('zone-tooltip-3d');
        if (tooltip) { tooltip.innerHTML = 'حالت جابجایی راهرو: نقطه شروع جدید را انتخاب کنید'; tooltip.style.display = 'block'; }

        // Register pointer-move handler for preview
        if (!corridorMouseMoveHandler) {
            corridorMouseMoveHandler = scene.onPointerObservable.add((pointerInfo) => {
                if (pointerInfo.type === BABYLON.PointerEventTypes.POINTERMOVE && corridorStartPoint) {
                    const pt = pickGroundPoint(pointerInfo);
                    if (pt) updateCorridorPreview(pt);
                }
            });
        }
    }

    // Cancel corridor relocate mode
    function cancelCorridorRelocate() {
        corridorRelocateMode = false;
        relocatingCorridorId = null;
        corridorStartPoint = null;

        if (corridorPreviewTube && !corridorPreviewTube.isDisposed()) corridorPreviewTube.dispose();
        corridorPreviewTube = null;
        if (corridorStartMarker && !corridorStartMarker.isDisposed()) corridorStartMarker.dispose();
        corridorStartMarker = null;
        if (corridorMouseMoveHandler) {
            scene.onPointerObservable.remove(corridorMouseMoveHandler);
            corridorMouseMoveHandler = null;
        }

        // Re-enable dragging on warehouse/zone boxes
        warehouseBoxes.forEach(({ mesh }) => {
            if (mesh) mesh.behaviors.forEach(b => { if (b instanceof BABYLON.PointerDragBehavior) b.enabled = true; });
        });
        zoneBoxes.forEach(({ mesh }) => {
            if (mesh) mesh.behaviors.forEach(b => { if (b instanceof BABYLON.PointerDragBehavior) b.enabled = true; });
        });

        if (camera) {
            camera.inputs.attached.pointers && (camera.inputs.attached.pointers.buttons = [0, 1, 2]);
        }

        const tooltip = document.getElementById('zone-tooltip-3d');
        if (tooltip) tooltip.style.display = 'none';
    }

    // Handle ground click during corridor relocate mode (reuses corridor draw logic)
    function handleCorridorRelocateClick(pointerInfo) {
        const pt = pickGroundPoint(pointerInfo);
        if (!pt) return;

        const tooltip = document.getElementById('zone-tooltip-3d');

        if (!corridorStartPoint) {
            corridorStartPoint = pt.clone();

            if (corridorStartMarker && !corridorStartMarker.isDisposed()) corridorStartMarker.dispose();
            corridorStartMarker = BABYLON.MeshBuilder.CreateSphere('corridorStart', { diameter: corridorDrawWidth * 0.8, segments: 8 }, scene);
            corridorStartMarker.position = new BABYLON.Vector3(corridorStartPoint.x, 0.3, corridorStartPoint.z);
            const mMat = new BABYLON.StandardMaterial('corridorStartMat', scene);
            mMat.diffuseColor = new BABYLON.Color3(0.9, 0.8, 0.1);
            mMat.emissiveColor = new BABYLON.Color3(0.4, 0.3, 0.0);
            corridorStartMarker.material = mMat;
            corridorStartMarker.isPickable = false;

            if (tooltip) { tooltip.innerHTML = 'نقطه شروع ثبت شد — نقطه پایان جدید را انتخاب کنید'; tooltip.style.display = 'block'; }
        } else {
            const endPoint = pt.clone();
            const from = corridorStartPoint;
            const cId = relocatingCorridorId;

            cancelCorridorRelocate();

            if (dotNetHelper) {
                dotNetHelper.invokeMethodAsync('OnCorridorRelocated', cId, from.x, from.z, endPoint.x, endPoint.z)
                    .catch(err => console.error('OnCorridorRelocated error:', err));
            }
        }
    }

    // ─── end corridor drawing ────────────────────────────────────────────────────

    // Toggle fullscreen on the container element
    function toggleFullscreen(containerId) {
        const container = document.getElementById(containerId);
        if (!container) return;

        if (!document.fullscreenElement) {
            container.requestFullscreen().then(() => {
                engine.resize();
            }).catch(err => console.error('Fullscreen error:', err));
        } else {
            document.exitFullscreen().then(() => {
                engine.resize();
            }).catch(err => console.error('Exit fullscreen error:', err));
        }
    }

    return {
        initialize: initialize,
        loadWarehouses: loadWarehouses,
        loadZones: loadZones,
        backToWarehouses: backToWarehouses,
        getPositions: getPositions,
        zoom: zoom,
        toggleFullscreen: toggleFullscreen,
        dispose: dispose,
        loadCorridors: loadCorridors,
        setCorridorDrawMode: setCorridorDrawMode,
        setCorridorWidth: setCorridorWidth,
        removeCorridorMesh: removeCorridorMesh,
        startCorridorRelocate: startCorridorRelocate,
        cancelCorridorRelocate: cancelCorridorRelocate
    };
})();
