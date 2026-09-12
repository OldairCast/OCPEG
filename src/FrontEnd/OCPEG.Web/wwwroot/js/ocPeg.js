(function () {
    this.ocPeg = this.ocPeg || {};
    var oc = this.ocPeg;

    oc.ErrorScript = function (obj, msg) {
        var msgErr = "Mensagem de Erro - " + obj + ": " + msg;

        console.log(msgErr);
        alert(msgErr);
    };



}());