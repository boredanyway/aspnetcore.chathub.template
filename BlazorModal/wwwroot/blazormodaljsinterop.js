export function initmodal(dotnetobjref) {

    var __obj = {

        modalmap: function(dotnetobjref) {

            this.showmodal = function(id) {

                $("#" + id).show(200);
            };
            this.hidemodal = function(id) {

                $("#" + id).hide(200);
            };
        },
    };

    return new __obj.modalmap(dotnetobjref);
}
