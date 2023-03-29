
$('.btnAddToCart').click(function (event) {
    event.preventDefault();
    var productid = Number($(this).data('id'));
    var quantity = 1;
    var tquantity = $('#quantity_value').text();
    if (tquantity != '') {
        quantity = parseInt(tquantity)
    }
    $.ajax({
        url: '/Menu/AddCart/',
        data: { productid: productid, quantity: quantity },
        type: 'POST',
        success: function (rs) {
            if (rs.success) {
                $('#checkout_items').html(rs.count)
            }
        }
    });
});




/* Set rates + misc */
var taxRate = 0;    // Thuế 0.05
var shippingRate = 0;   // Ship 15.000
var fadeTime = 300;


/* Assign actions */
$('.product-quantity input').change(function () {
    updateQuantity(this);
});

$('.product-removal button').click(function () {
        removeItem(this);
    var productid = Number($(this).data('id'));
    $.ajax({
        url: '/Menu/RemoveCart/',
        type: 'POST',
        data: { productid: productid },
        success: function (rs) {
            if (rs.success) {
                //alert(rs.count)
                $('#checkout_items').html(rs.count)
                //alert(rs.msg)
            }

        }
    })


});


/* Recalculate cart */
function recalculateCart() {
    var subtotal = 0;

    /* Sum up row totals */
    $('.product').each(function () {
        subtotal += parseFloat($(this).children('.product-line-price').text());
        //subtotal += parseFloat($(this).children('.product-line-price').text()).toFixed(2);
        
    });

    /* Calculate totals */
    var tax = subtotal * taxRate;
    var shipping = (subtotal > 0 ? shippingRate : 0);
    var total = subtotal + tax + shipping;

    /* Update totals display */
    $('.totals-value').fadeOut(fadeTime, function () {
        $('#cart-subtotal').html(subtotal.toFixed(2));
        $('#cart-tax').html(tax.toFixed(2));
        $('#cart-shipping').html(shipping.toFixed(2));
        $('#cart-total').html(total.toFixed(2));
        if (total == 0) {
            $('.checkout').fadeOut(fadeTime);
        } else {
            $('.checkout').fadeIn(fadeTime);
        }
        $('.totals-value').fadeIn(fadeTime);
    });
}


/* Update quantity */
function updateQuantity(quantityInput) {
    /* Calculate line price */
    var productid = $(quantityInput).data('id');
    var productRow = $(quantityInput).parent().parent();

    var price = parseFloat(productRow.children('.product-price').text()).toFixed(2);

    var quantity = $(quantityInput).val();

    $.ajax({
        url: '/Menu/UpdateCart/',
        type: 'POST',
        data: { productid: productid, quantity: quantity },
        success: function (rs) {
            if (rs.success) {
                //alert(rs.msg)
            }
        }
    })

    var linePrice = price * quantity;

    /* Update line price display and recalc cart totals */
    productRow.children('.product-line-price').each(function () {
        $(this).fadeOut(fadeTime, function () {
            $(this).text(linePrice.toFixed(2));
            recalculateCart();
            $(this).fadeIn(fadeTime);
        });
    });
}


/* Remove item from cart */
function removeItem(removeButton) {
    /* Remove row from DOM and recalc cart total */
    var productRow = $(removeButton).parent().parent();
    productRow.slideUp(fadeTime, function () {
        productRow.remove();
        recalculateCart();
    });
}