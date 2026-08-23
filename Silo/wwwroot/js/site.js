function removeAttr(selector, attr) {
    $(selector).removeAttr(attr);
};

function scrollToBottom() {
    window.scrollTo({ top: document.body.scrollHeight, behavior: 'smooth' });
};

(function () {
    var stored = localStorage.getItem('silo-theme');
    var isDark = false;
    if (stored) {
        isDark = stored === 'dark';
        document.documentElement.setAttribute('data-bs-theme', stored);
    } else if (window.matchMedia && window.matchMedia('(prefers-color-scheme: dark)').matches) {
        isDark = true;
        document.documentElement.setAttribute('data-bs-theme', 'dark');
    }
    siloSetTheme(isDark, false);
})();

function siloSetTheme(isDark, reloadRequired) {
    var theme = isDark ? 'dark' : 'light';
    document.documentElement.setAttribute('data-bs-theme', theme);
    var light = document.getElementById('telerik-theme-light');
    var dark = document.getElementById('telerik-theme-dark');
    if (light) light.media = isDark ? 'not all' : 'all';
    if (dark) dark.media = isDark ? 'all' : 'not all';
    localStorage.setItem('silo-theme', theme);
    if (reloadRequired) {
        window.location.reload();
    }
}

function loadScript(id, src) {
    return new Promise(function (resolve, reject) {
        if (typeof Warehouse3D !== 'undefined') { resolve(); return; }
        var existing = document.getElementById(id);
        if (existing) { existing.addEventListener('load', resolve); existing.addEventListener('error', reject); return; }
        var s = document.createElement('script');
        s.id = id;
        s.src = src;
        s.onload = resolve;
        s.onerror = reject;
        document.head.appendChild(s);
    });
}

function removeScript(id) {
    var s = document.getElementById(id);
    if (s) s.remove();
}

$(document).ready(function () {
    $('.text-dir-left .k-input-inner').change(function () {
        $(this).removeAttr('dir');
    });
});