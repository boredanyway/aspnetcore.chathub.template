export function initmodal(dotnetobjref) {

    var __obj = {

        modalmap: function(dotnetobjref) {

            this.showmodal = function(id) {

                $("#"+id).modal("show");
            };
            this.hidemodal = function(id) {

                $("#"+id).modal("hide");
            };
        },
    };

    return new __obj.modalmap(dotnetobjref);
}
