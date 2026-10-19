// Keyboard shortcuts of the batch page (legacy form): F5 clears the entry fields, F6 saves, F7 opens the print page.
// F5 would reload the page, so it is intercepted. Everything else is left alone.
const keys = new Set(['F5', 'F6', 'F7']);
let handler = null;

export function register(dotNetRef) {
    unregister();
    handler = (event) => {
        if (!keys.has(event.key) || event.ctrlKey || event.altKey || event.metaKey || event.shiftKey) {
            return;
        }
        event.preventDefault();
        dotNetRef.invokeMethodAsync('OnShortcut', event.key);
    };
    document.addEventListener('keydown', handler);
}

export function unregister() {
    if (handler) {
        document.removeEventListener('keydown', handler);
        handler = null;
    }
}
