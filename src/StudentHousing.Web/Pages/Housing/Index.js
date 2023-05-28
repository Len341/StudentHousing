
$(document).ready(function () {

    createDt();

    $('#AddPropertyButton').click(function () {
        propertyModal.open();
    })

})

var propertyModal = new abp.ModalManager({
    viewUrl: abp.appPath + 'Housing/PropertyModal',
    modalClass: 'Property'
});

propertyModal.onResult(function () {

    DataTable.ajax.reload();
    abp.notify.info('Success!');
})

var responseCallback = function (result) {

    abp.ui.clearBusy();

    return {
        recordsTotal: result.totalCount,
        recordsFiltered: result.totalCount,
        data: result
    };
}

abp.modals.Property = function () {

    function initModal(modalManager, args) {
        var $modal = modalManager.getModal();
        var $form = $('form');

        if ($form.find('#PropertyId').val() != '') {

            let slideIndex = 1;
            showSlides(slideIndex);
        }

    }

    return {
        initModal: initModal
    };
}

var inputAction = function () {
    abp.ui.setBusy('#PropertiesTable');
    return {};
}

var DataTable = null;
function createDt() {

    DataTable = $('#PropertiesTable').DataTable(
        abp.libs.datatables.normalizeConfiguration({
            //dom: 'B',
            serverSide: false,
            paging: true,
            searching: true,
            scrollX: false,
            processing: false,
            responsive: true,
            order: [],
            lengthMenu: [[7, 14, 21, 50], [7, 14, 21, 50]],
            ajax: abp.libs.datatables.createAjax(studentHousing.housing.housing.getList, inputAction, responseCallback),
            columnDefs: [
                {
                    title: "Actions",
                    rowAction: {
                        items:
                            [
                                {
                                    text: "View",
                                    action: function (data) {
                                        propertyModal.open({ propertyId: data.record.id });
                                    }
                                },
                                {
                                    text: "Add Registration Number",
                                    visible: abp.auth.isGranted('StudentHousing.Housing.Create') || abp.auth.isGranted('StudentHousing.Housing.Update'),
                                    action: function (data) {
                                        //todo
                                    }
                                },
                                {
                                    text: "Delete",
                                    visible: abp.auth.isGranted('StudentHousing.Housing.Delete'),
                                    confirmMessage: function (data) {
                                        return 'Are you sure you want to delete this propery?';
                                    },
                                    action: function (data) {
                                        studentHousing.housing.housing.delete(data.record.id)
                                            .then(function () {
                                                abp.notify.info('Property Successfully Deleted');
                                                DataTable.ajax.reload();
                                            });
                                    }
                                }
                            ]
                    }
                },
                {
                    title: "Property Name",
                    data: "name"
                }
            ]
        })
    );

}
