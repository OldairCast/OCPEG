(function () {
    this.ocPegCall = this.ocPegCall || {};
    var call = this.ocPegCall;

    /// <summary> Pesquisa Cliente </summary>
    call.CustomerSearch = function (url) {
        try {
            var url = sessionStorage.getItem('Url') + "Customer/GetByName";
            var token = $('input[name="__RequestVerificationToken"]').val();

            var request = $.ajax({
                type: "POST",
                data: { __RequestVerificationToken: token, name: $("#CustomerName").val() },
                url: url,
                cache: false,
                beforeSend: function () {
                    //window.oc.Wait(true);
                }
            });
            request.done(function (response) {
                if (response.status == 2) {
                    //Erro
                    alert(response.message);
                } else {
                    call.CustomerReturn(response);
                }
            });
            request.fail(function (xhr, textStatus) {
                alert(xhr.statusText + " - " + textStatus);
            });

        }
        catch (err) {
            window.exceller.ErrorScript("call.CustomerSearch", err.message);
        }
    };

    /// <summary> Pesquisa Cliente Retorno </summary>
    call.CustomerReturn = function (response) {
        try {
            $("#Customer_Id").empty();

            var customers = response;

            if (customers == null || customers.length == 0) {
                alert("Nenhum cliente encontrado !");
                return;
            }

            if (customers.length == 1) {
                $.each(customers, function (i, item) {
                    $('#Customer_Id').append($('<option>', {
                        value: item.id,
                        text: item.name
                    }));
                });
            }
            else {
                $('#Customer_Id').append($('<option>', {
                    value: 0,
                    text: '--- Selecionar ---'
                }));

                $.each(customers, function (i, item) {
                    $('#Customer_Id').append($('<option>', {
                        value: item.id,
                        text: item.name
                    }));
                });
            }

            $("#divCustomerSearch").css("display", "none");
            $("#divCustomer").css("display", "inline-block");

        }
        catch (err) {
            window.exceller.ErrorScript("call.CustomerReturn", err.message);
        }
    };

    /// <summary> Retorna para nova pesquisa de cliente </summary>
    call.CustomerBack = function () {
        try {
            $("#CustomerId").empty();

            $("#divCustomerSearch").css("display", "block");
            $("#divCustomer").css("display", "none");

        }
        catch (err) {
            window.exceller.ErrorScript("call.CustomerBack", err.message);
        }
    };
}());


