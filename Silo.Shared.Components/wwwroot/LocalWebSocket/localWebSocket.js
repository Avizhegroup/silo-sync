let socket;
let dotNetRef;
let retryTimer;

function notify(method, ...args) {
    dotNetRef?.invokeMethodAsync(method, ...args).catch(() => { });
}

// attempts: how many times to try before reporting a failure (the host accepts one browser at a time, so a
// reconnect right after leaving another page can be refused for a moment until the old socket has closed).
export function connect(url, reference, attempts = 1, delayMilliseconds = 750) {
    dotNetRef = reference;
    if (socket?.readyState === WebSocket.OPEN || socket?.readyState === WebSocket.CONNECTING) {
        notify("OnSocketStateChanged", socket.readyState === WebSocket.OPEN);
        return;
    }

    clearTimeout(retryTimer);
    open(url, Math.max(1, attempts), delayMilliseconds);
}

function open(url, remaining, delayMilliseconds) {
    const current = new WebSocket(url);
    socket = current;
    let opened = false;

    current.addEventListener("open", () => {
        opened = true;
        notify("OnSocketStateChanged", true);
    });
    current.addEventListener("close", () => {
        if (socket !== current) {
            return; // closed on purpose by disconnect() or replaced by a newer socket
        }
        socket = undefined;

        if (opened) {
            notify("OnSocketStateChanged", false);
            return;
        }

        if (remaining > 1) {
            retryTimer = setTimeout(() => {
                if (!socket && dotNetRef) {
                    open(url, remaining - 1, delayMilliseconds);
                }
            }, delayMilliseconds);
            return;
        }

        notify("OnSocketError", "اتصال به برنامه‌ی دستگاه برقرار نشد. مطمئن شوید برنامه اجرا شده و رمز درست است.");
        notify("OnSocketStateChanged", false);
    });
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
    clearTimeout(retryTimer);
    const current = socket;
    socket = undefined;
    dotNetRef = undefined;
    current?.close(1000, "Component disconnected");
}
