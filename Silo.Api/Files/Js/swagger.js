(function () {
    window.addEventListener("load", function () {
        setTimeout(function () {
            var logo = document.getElementsByClassName('link');
            logo[0].href = "https://avizhegroup.com/";
            logo[0].target = "_blank";
            logo[0].children[0].alt = "Implementing Swagger";
            logo[0].children[0].src = "/Files/Images/Avizhe Logo.png";
            logo[0].append('RfidCore');
            document.getElementsByTagName('title')[0].text = 'Avizhegroup | Rfidcore doc';

            var link = document.querySelector("link[rel~='icon']");
            if (!link) {
                link = document.createElement('link');
                link.rel = 'icon';
                document.getElementsByTagName('head')[0].appendChild(link);
            }
            link.href = '/Files/favicon.ico';
        });
    });
})();