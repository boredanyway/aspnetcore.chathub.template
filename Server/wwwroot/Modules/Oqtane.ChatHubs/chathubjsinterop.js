export function initchathub() {

    var __obj = {

        chathubmap: function () {

            this.showchathubscontainer = function () {

                var chathubscontainer = document.querySelector('.chathubs-container');
                chathubscontainer.style.transition = "opacity 0.24s";
                chathubscontainer.style.opacity = "1";
            };
        }
    }

    return new __obj.chathubmap();
}