var Candidates = (function () {
    function Candidates() {

    }

    Candidates.Init = function () {
       
        $('#Birthdate').mask('99/99/9999', { placeholder: "__/__/____" });
        var options = { language: 'esp', format: "dd/mm/yyyy" };
        $('.dp-date input').datepicker(options);        

        $("#Birthdate").val($("#hdf_Birthdate").val());
    };

    //Método utilizado para abrir o cerrar los detalles.
    Candidates.CollapseOrExpand = function (idCandidate) {

        var isExpanded = $(`#iconCollapse_${idCandidate}`).data('isexpanded');

        if (isExpanded === 'true') {
            $(`#iconCollapse_${idCandidate}`).removeClass('fa fa-chevron-down');
            $(`#iconCollapse_${idCandidate}`).addClass('fa fa-chevron-right');
            $(`#divParent_${idCandidate}`).fadeOut(300);

        } else {
            $(`#iconCollapse_${idCandidate}`).removeClass('fa fa-chevron-right');
            $(`#iconCollapse_${idCandidate}`).addClass('fa fa-chevron-down');
            $(`#divParent_${idCandidate}`).fadeIn(500);

            showLoading();

            var urlReload = $("#urlCandidateExperience").val().replace('0', idCandidate);

            Petitions.Get(urlReload, function (response) {
                $(`#divChildren_${idCandidate}`).html(response);
                CreateAllTooltips();
                hideLoading();
            });

        }

        $(`#iconCollapse_${idCandidate}`).data('isexpanded', (isExpanded === 'true') ? 'false' : 'true');
    };

    //Método encargado de abrir la ventana para editar un candidato
    Candidates.OpenEdit = function (urlEdit) {
        Petitions.Get(urlEdit, function (response) {
            $('#modal-partial-content').html(response);
            showModalResult();
        });
    };

    //Método encargado de recargar la información de los candidatos
    Candidates.Reload = function () {
        new MvcGrid(document.querySelector('.mvc-grid')).reload();
    };

    //Método encargado de recargar la tabla de experiencia de un candidato.
    Candidates.ReloadExperiences = function (urlLoad, candidateId) {
        Petitions.Get(urlLoad, function (response) {
            $(`#divChildren_${candidateId}`).html(response);
            CreateAllTooltips();
        });
    };

    //Método encargado de realizar el borrado de un candidato.
    Candidates.Delete = function (url) {
        ShowModalQuestion('Advertencia', '¿Desea eliminar este registro?', function () {
            Petitions.Get(url, function (response) {
                HideModalQuestion();

                if (response.Success) {
                    showAlert(response.Message, 'success');
                    Candidates.Reload();
                } else {
                    showAlert(response.Message);
                }
            });
        });
    };

    Candidates.OpenCreate = function (url) {
        Petitions.Get(url, function (response) {
            $('#modal-partial-content').html(response);
            showModalResult();
        });
    };


    Candidates.SaveCandidate = function () {
        var candidate = getObject("formCandidates");

        if (formValidation("formCandidates")) {
            ExecuteAjax("Candidates/CreateOrUpdate", "Post", candidate, "Candidates.ResultSaveCandidate");
        }
    };

    Candidates.ResultSaveCandidate = function (response) {
        if (response.Success) {
            showAlert(response.Message, 'success');
            hideModalPartial();
            Candidates.Reload();
        } else {
            showAlert(response.Message, 'danger');
        }        
    };

    return Candidates;

})();



