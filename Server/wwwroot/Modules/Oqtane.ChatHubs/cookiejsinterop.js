export function initcookie() {

    var __obj = {

        cookiemap: function () {

            this.getCookie = function (cname) {
                var name = cname + "=";
                var ca = document.cookie.split(';');
                for (var i = 0; i < ca.length; i++) {
                    var c = ca[i];
                    while (c.charAt(0) == ' ') {
                        c = c.substring(1);
                    }
                    if (c.indexOf(name) == 0) {
                        return c.substring(name.length, c.length);
                    }
                }
                return "";
            };
            this.setCookie = function (cname, cvalue, expirationdays) {
                var d = new Date();
                d.setTime(d.getTime() + (expirationdays * 24 * 60 * 60 * 1000));
                var expires = "expires=" + d.toUTCString();
                document.cookie = cname + "=" + cvalue + ";" + expires + ";path=/";
            };
        }
    };

    return new __obj.cookiemap();
}