let socket;
const handlers = new Set();

export function connect(url) {
    if (socket?.readyState === WebSocket.OPEN || socket?.readyState === WebSocket.CONNECTING) {
        return;
    }
    socket = new WebSocket(url);
    window.localWebSocket = {
        send,
        isConnected,
        onMessage,
        offMessage,
        disconnect
    };
    socket.addEventListener("message", event => {
        if (typeof event.data !== "string") {
            socket.close(1003, "Text messages only");
            return;
        }
        handlers.forEach(handler => handler(event.data));
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

export function onMessage(callback) {
    handlers.add(callback);
}

export function offMessage(callback) {
    handlers.delete(callback);
}

export function disconnect() {
    handlers.clear();
    socket?.close(1000, "Component disconnected");
    socket = undefined;
    delete window.localWebSocket;
}
