let socket;
let dotNetRef;

function notify(method, ...args) {
    dotNetRef?.invokeMethodAsync(method, ...args).catch(() => { });
}

export function connect(url, reference) {
    dotNetRef = reference;
    if (socket?.readyState === WebSocket.OPEN || socket?.readyState === WebSocket.CONNECTING) {
        notify("OnSocketStateChanged", socket.readyState === WebSocket.OPEN);
        return;
    }

    const current = new WebSocket(url);
    socket = current;
    current.addEventListener("open", () => notify("OnSocketStateChanged", true));
    current.addEventListener("close", () => {
        if (socket === current) {
            socket = undefined;
        }
        notify("OnSocketStateChanged", false);
    });
    current.addEventListener("error", () => notify("OnSocketError", "Connection to the local device host failed."));
    current.addEventListener("message", event => {
        if (typeof event.data !== "string") {
            current.close(1003, "Text messages only");
            return;
        }
        notify("OnSocketMessage", event.data);
    });
}

export function send(data) {
    if (typeof data !== "string") {
        throw new TypeError("Only text messages are supported.");
    }
    if (socket?.readyState !== WebSocket.OPEN) {
        throw new Error("The local WebSocket is not connected.");
    }
    socket.send(data);
}

export function isConnected() {
    return socket?.readyState === WebSocket.OPEN;
}

export function disconnect() {
    const current = socket;
    socket = undefined;
    dotNetRef = undefined;
    current?.close(1000, "Component disconnected");
}
