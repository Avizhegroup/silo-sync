window.scrollToBottom = (element) => {
    if (element) {
        element.scrollTop = element.scrollHeight;
    }
};

window.copyToClipboard = (text) => {
    if (navigator.clipboard && navigator.clipboard.writeText) {
        return navigator.clipboard.writeText(text);
    }
    const textarea = document.createElement('textarea');
    textarea.value = text;
    textarea.style.position = 'fixed';
    textarea.style.opacity = '0';
    document.body.appendChild(textarea);
    textarea.select();
    document.execCommand('copy');
    document.body.removeChild(textarea);
    return Promise.resolve();
};

window.downloadChartAsPng = async (container, fileName) => {
    if (!container) {
        return;
    }

    const svg = container.querySelector("svg");
    if (!svg) {
        return;
    }
    const scrollHost = svg.closest(".ai-chart-scroll-x, .ai-chart-scroll-y") || svg.parentElement;

    const rect = svg.getBoundingClientRect();

    const width = Math.ceil(Math.max(
        rect.width,
        svg.scrollWidth,
        svg.parentElement?.scrollWidth || 0,
        scrollHost?.scrollWidth || 0
    ));

    const height = Math.ceil(Math.max(
        rect.height,
        svg.scrollHeight,
        svg.parentElement?.scrollHeight || 0
    ));

    if (!width || !height) {
        return;
    }

    const padding = 40;

    const totalWidth = width + padding * 2;

    const totalHeight = height + padding * 2;

    const clonedSvg = svg.cloneNode(true);

    clonedSvg.setAttribute("xmlns", "http://www.w3.org/2000/svg");
    clonedSvg.setAttribute("width", width);
    clonedSvg.setAttribute("height", height);
    clonedSvg.setAttribute("viewBox", `0 0 ${width} ${height}`);

    clonedSvg.style.overflow = "visible";

    const styleProperties = [
        "fill", "fill-opacity", "stroke", "stroke-width", "stroke-opacity",
        "stroke-dasharray", "stroke-linecap", "stroke-linejoin",
        "font-family", "font-size", "font-weight", "font-style",
        "text-anchor", "dominant-baseline", "direction", "unicode-bidi",
        "letter-spacing", "white-space", "opacity", "visibility"
    ];

    const originalElements = [svg, ...svg.querySelectorAll("*")];
    const clonedElements = [clonedSvg, ...clonedSvg.querySelectorAll("*")];

    originalElements.forEach((el, i) => {
        const cs = window.getComputedStyle(el);
        const target = clonedElements[i];

        styleProperties.forEach(p => {
            const v = cs.getPropertyValue(p);
            if (v) {
                target.style.setProperty(p, v);
            }
        });
    });

    const rootFont = window.getComputedStyle(container).fontFamily;

    clonedSvg.style.fontFamily = rootFont;

    let bg = "#ffffff";

    let el = svg;

    while (el) {
        const c = window.getComputedStyle(el).backgroundColor;

        if (c && c !== "rgba(0, 0, 0, 0)" && c !== "transparent") {
            bg = c;
            break;
        }

        el = el.parentElement;
    }

    const svgString = new XMLSerializer().serializeToString(clonedSvg);

    const svgBlob = new Blob([svgString], { type: "image/svg+xml;charset=utf-8" });

    const svgUrl = URL.createObjectURL(svgBlob);

    const image = new Image();

    await new Promise((resolve, reject) => {
            image.onload = resolve;
            image.onerror = reject;
            image.src = svgUrl;
        });

    const scale = 2;

    const canvas = document.createElement("canvas");

    canvas.width = totalWidth * scale;

    canvas.height = totalHeight * scale;

    const ctx = canvas.getContext("2d");

    if (!ctx) {
        return;
    }

    ctx.fillStyle = bg;

    ctx.fillRect(0, 0, canvas.width, canvas.height);

    ctx.scale(scale, scale);

    ctx.drawImage(image, padding, padding, width, height);

    const pngBlob = await new Promise(resolve => canvas.toBlob(resolve, "image/png"));

    if (!pngBlob) {
        return;
    }

    const pngUrl = URL.createObjectURL(pngBlob);

    const link = document.createElement("a");

    link.href = pngUrl;

    link.download = fileName || "Chart.png";

    document.body.appendChild(link);

    link.click();

    link.remove();

    setTimeout(() => URL.revokeObjectURL(pngUrl), 1000);
  
     URL.revokeObjectURL(svgUrl);
    
};