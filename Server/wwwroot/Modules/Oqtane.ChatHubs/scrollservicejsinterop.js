export function initscrollservice() {

    var __obj = {

        scrollmap: function () {

            this.scrollToBottom = function (elementId) {

                try {

                    var messagewindow = document.querySelector(elementId);
                    if (messagewindow !== null) {

                        var lastchild = messagewindow.lastElementChild;
                        var lastchildheight = lastchild.offsetHeight;
                        var tolerance = 100;

                        if (messagewindow.scrollTop + messagewindow.offsetHeight + lastchildheight + tolerance >= messagewindow.scrollHeight) {

                            messagewindow.scrollTo({
                                top: messagewindow.scrollHeight,
                                left: 0,
                                behavior: 'smooth'
                            });
                        }
                    }
                }
                catch (err) {

                    console.warn(err);
                }
            };
        }
    }

    return new __obj.scrollmap();
}