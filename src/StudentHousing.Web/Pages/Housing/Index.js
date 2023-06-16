var price = '';
var distance = '';
var currentUserId = abp.currentUser.id;
var propertyId;

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

var regNoModal = new abp.ModalManager({
    viewUrl: abp.appPath + 'Housing/RegistrationNumberModal',
    modalClass: 'RegistrationNumber'
});

var propertyEnquiryModal = new abp.ModalManager({
    viewUrl: abp.appPath + 'Housing/PropertyEnquiryModal',
    modalClass: 'PropertyEnquiry'
});

propertyEnquiryModal.onResult(function () {
    $('div:first').show();
    abp.notify.info('Property enquiry has been sent!');
});
propertyEnquiryModal.onClose(function () {
    $('div:first').show();
});

regNoModal.onResult(function () {
    DataTable.ajax.reload();
    abp.notify.info('Success!');
});

propertyModal.onResult(function () {
    DataTable.ajax.reload();
    abp.notify.info('Success!');
});

var responseCallback = function (result) {

    abp.ui.clearBusy();

    return {
        recordsTotal: result.totalCount,
        recordsFiltered: result.totalCount,
        data: result
    };
}

var enquiryResponseCallback = function (result) {

    result = result.filter(z => z.propertyId == propertyId);

    abp.ui.clearBusy();

    return {
        recordsTotal: result.totalCount,
        recordsFiltered: result.totalCount,
        data: result
    };
}

function search() {
    price = $('#priceSelect').find('option:selected').val();
    distance = $('#distanceSelect').find('option:selected').val();
    DataTable.ajax.reload();
}

abp.modals.Property = function () {

    function initModal(modalManager, args) {
        var $modal = modalManager.getModal();
        var $form = $('form');

        if ($form.find('#PropertyId').length) {
            let slideIndex = 1;
            showSlides(slideIndex);
            propertyId = $form.find('#PropertyId').val();

            if ($('#PropertyEnquiriesTable').length) {
                setTimeout(function () {
                    var dt = $('#PropertyEnquiriesTable').DataTable(
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
                            ajax: abp.libs.datatables.createAjax(studentHousing.housing.housing.getPropertyEnquiryList, inputAction, enquiryResponseCallback),
                            columnDefs: [
                                {
                                    title: "Student Name",
                                    data: "studentName"
                                },
                                {
                                    title: "Student Email",
                                    data: "studentEmail"
                                },
                                {
                                    title: "Student Number",
                                    data: "studentNumber"
                                },
                                {
                                    title: "Student Message",
                                    data: "messageFromStudent"
                                }
                            ]
                        })
                    );
                }, 1000)
            }

            $('#propEnquire').click(function () {
                $('div:first').hide();
                propertyEnquiryModal.open({ studentId: currentUserId, propertyId: propertyId });
            });

        }

    }

    return {
        initModal: initModal
    };
}

var inputAction = function () {
    abp.ui.setBusy('#PropertiesTable');
    return { distance: distance, price: price };
}

var enquiryInputAction = function () {
    abp.ui.setBusy('#PropertyEnquiriesTable');
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
                                        regNoModal.open({ propertyId: data.record.id });
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
                },
                {
                    title: "Distance From University",
                    data: "distanceFromUniversity",
                    render: function (data) {
                        if (data != null && data != undefined && data != 0) {
                            return (data / 1000).toFixed(2) + "km"
                        } else {
                            return "Unknown";
                        }

                    }
                },
                {
                    title: "Monthly Price",
                    data: "monthlyPrice",
                    render: function (data) {
                        return "R" + data.toString();
                    }
                }
            ]
        })
    );

}
