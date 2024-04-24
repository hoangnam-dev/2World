// Handle show modal when click button "Delete" 
$(document).ready(function () {
    $('.btnDelete').click(function () {
        // Lấy giá trị của thuộc tính data-id của nút được nhấp
        var dataId = $(this).data('id');
        // Sử dụng giá trị dataId ở đây, ví dụ: in ra console
        console.log("data-id của nút được nhấp là: " + dataId);
        $('#itemId').val(dataId);
    });

    $('.page-item').click(function () {
        $('.page-item').removeClass('active');

        $(this).addClass('active');
    });
    $('.showPW').click(function () {
        var targetField = $('#' + $(this).data('target'));

        if (targetField.attr('type') === "password") {
            targetField.attr('type', 'text');
            $(this).html('<i class="fa-solid fa-eye-slash"></i>');
        } else {
            targetField.attr('type', 'password');
            $(this).html('<i class="fa-solid fa-eye"></i>');
        }
    });

    $('#passwordField, #retypePasswordField').keyup(checkRetypePw);

    $('#customerForm').submit(function (e) {
        e.preventDefault();
        var isValid = true;
        var name = $('.customerName').val().trim();
        var email = $('.customerEmail').val().trim();
        var phone = $('.customerPhone').val().trim();
        var address = $('.customerAddress').val().trim();

        console.log(name + ' === ' + email + ' === ' + phone + ' === ' +address)
        if (name == '' || email == '' || phone == '' || address == '') {
            isValid = false;
            $('.formMsg').html('Please complete all information')
        }
        console.log(isValid);
        console.log(checkRetypePw());
        if (isValid && checkRetypePw()) {
            $('#customerCreateForm').unbind('submit').submit();
        }
    });

    $('#customerEditForm').submit(function (e) {
        e.preventDefault();
        if (checkRetypePw()) {
            $('#customerEditForm').unbind('submit').submit();
        }
    });


    function checkRetypePw() {
        var pw = $('#passwordField').val();
        var retypePW = $('#retypePasswordField').val();
        $('#message').removeClass();

        if (pw != retypePW) {
            $('#message').html('Password do not match.').addClass('text-danger');
            return false;
        } else {
            $('#message').html('Password matched').addClass('text-success');
        }
        return true
    }

    //$('#retypePasswordField').keyup(function (e) {
    //    e.preventDefault()
    //    var pw = $('#passwordField').val();
    //    var retypePW = $('#retypePasswordField').val();
    //    $('#message').removeClass();

    //    if (pw != retypePW) {
    //        $('#message').html('Password do not match.').addClass('text-danger');
    //        console.log('false');
    //    } else {
    //        $('#message').html('Password matched').addClass('text-success');
    //    }
    //    console.log('oke');
    //});
})

