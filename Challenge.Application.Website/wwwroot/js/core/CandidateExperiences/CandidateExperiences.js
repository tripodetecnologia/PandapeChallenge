var CandidateExperiences = (function () {
    function CandidateExperiences() {

    }

    CandidateExperiences.Init = function () {

        $('#BeginDate').mask('99/99/9999', { placeholder: "__/__/____" });
        $('#EndDate').mask('99/99/9999', { placeholder: "__/__/____" });

        var options = { language: 'esp', format: "dd/mm/yyyy" };
        $('.dp-date input').datepicker(options);

        $("#BeginDate").val($("#hdf_BeginDate").val());
        $("#EndDate").val($("#hdf_EndDate").val());

    };

    //Método encargado de abrir la ventana para editar una experiencia
    CandidateExperiences.OpenEditExperiences = function (urlEdit) {
        Petitions.Get(urlEdit, function (response) {
            $('#modal-partial-content').html(response);
            showModalResult('', true);
        });
    };

    //Método encargado de recargar la tabla de experiencia de un candidato.
    CandidateExperiences.ReloadExperiences = function (urlLoad, candidateId) {
        Petitions.Get(urlLoad, function (response) {
            $(`#divChildren_${candidateId}`).html(response);
            CreateAllTooltips();
        });
    };

    //Método encargado de borrar una experiencia
    CandidateExperiences.DeleteExperience = function (url, candidateId) {
        ShowModalQuestion('Advertencia', '¿Desea eliminar este registro?', function () {
            HideModalQuestion();

            Petitions.Get(url, function (response) {

                if (response.Success) {
                    showAlert(response.Message, 'success');
                    CandidateExperiences.ReloadExperience(candidateId);
                } else {
                    showAlert("Ocurrió un error eliminando la información, verifique que no tenga datos asociados.");
                }
            });
        });
    };

    CandidateExperiences.OpenCreate = function (url) {
        Petitions.Get(url, function (response) {
            $('#modal-partial-content').html(response);
            showModalResult();
        });
    };
       
    CandidateExperiences.SaveExperience = function () {
        var candidateExperience = getObject("formExperience");

        if (formValidation("formExperience")) {
            ExecuteAjax("CandidateExperiences/CreateOrUpdate", "Post", candidateExperience, "CandidateExperiences.ResultSaveExperience");
        }
    };

    CandidateExperiences.ResultSaveExperience = function (response) {
        if (response.Success) {
            showAlert(response.Message, 'success');
            hideModalPartial();
            CandidateExperiences.ReloadExperience($("#IdCandidate").val());
        } else {
            showAlert(response.Message, 'danger');
        }

    };

    CandidateExperiences.ReloadExperience = function (candidateId) {
        var urlReload = $("#urlCandidateExperience").val().replace('0', candidateId);        
        CandidateExperiences.ReloadExperiences(urlReload, candidateId);
    };

    return CandidateExperiences;
})();



