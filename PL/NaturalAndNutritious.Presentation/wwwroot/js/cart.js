let baseUrl = window.location.origin;
async function getData(ids) {
    const promises = ids.map((id) => fetch(`${baseUrl}/Products/GetProductsByIds?id=${id}`))
    const datas =  await Promise.all(promises).then(responses => Promise.all(responses.map(r => r.json())))
    return datas
}

function sendData(data)
{
    fetch(`${baseUrl}/Cart/ProceedCheckout`,
        {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(data)
        })
        .then(dt => dt.json()).then((dt) => {
            if (dt.succeeded) {
                window.location.href = `${baseUrl}/Checkout`;
            }
        })
        .catch(err => console.log(err));
}

function removeProduct(productId) {
    // LocalStorage'dan ürünleri alın
    let products = JSON.parse(localStorage.getItem('cartProducts')) || [];

    // Ürünü diziden kaldırın
    let newPRoducts = products.filter(product => product.id !== productId);

    // Güncellenmiş ürün dizisini tekrar LocalStorage'a kaydedin
    localStorage.setItem('cartProducts', JSON.stringify(newPRoducts));

    // Kullanıcıya bir bildirim gösterebilirsiniz veya ürün listesini güncelleyebilirsiniz
    renderProducts().then(() => {
        renderTotal();
        alert("Product deleted from the cart");
    });
}

window.addEventListener('DOMContentLoaded',async () => {

    await renderProducts();
    renderTotal();
    setEventListeners();
}
);


async function renderProducts() {
    let storage = localStorage.getItem('cartProducts');
    if (!storage)
        return;

    let cartProducts = Array.from(JSON.parse(storage));

    const ids = cartProducts.map((prod) => prod.id)
    const myCardPorducts = await getData(ids)


    const productsBody = document.getElementById('products-body');
    productsBody.innerHTML = '';
    myCardPorducts.forEach(pd => {
        let quantity = parseInt(cartProducts.find(cp => cp.id == pd.id).quantity);
        productsBody.innerHTML += `
                                <tr class="product-data" id='data_${pd.id}'>
                        <th scope="row">
                            <div class="d-flex align-items-center">
                                <img src="${pd.photo}" id='photo_${pd.id}' class="img-fluid me-5 rounded-circle" style="width: 80px; height: 80px;" alt="">
                            </div>
                        </th>
                        <td>
                            <p class="mb-0 mt-4" id="name_${pd.id}">${pd.name}</p>
                        </td>
                        <td>
                            <p class="mb-0 mt-4" id='price_${pd.id}'>${pd.price} ₼</p>
                        </td>
                        <td>
                            <div class="input-group quantity mt-4" style="width: 100px;">
                                <div class="input-group-btn">
                                    <button id='btnMin_${pd.id}' class="btn btn-sm btn-minus rounded-circle bg-light border">
                                        <i class="fa fa-minus"></i>
                                    </button>
                                </div>
                                <input type="text" class="form-control form-control-sm text-center border-0 product-quantity" id='quant_${pd.id}' value="${quantity}">
                                <div class="input-group-btn">
                                    <button id='btnPls_${pd.id}' class="btn btn-sm btn-plus rounded-circle bg-light border">
                                        <i class="fa fa-plus"></i>
                                    </button>
                                </div>
                            </div>
                        </td>
                        <td>
                            <p class="mb-0 mt-4 totalPrice" id='total_${pd.id}'>${quantity * pd.price} ₼</p>
                        </td>
                        <td>
                            <button onclick="removeProduct('${pd.id}')" class="btn btn-md rounded-circle bg-light border mt-4">
                                <i class="fa fa-times text-danger"></i>
                            </button>
                        </td>

                    </tr>
        `
    });

}



function setEventListeners() {
    let priceSum = 0;
    document.querySelectorAll('.totalPrice').forEach(tp => {
        priceSum += parseInt(tp.innerText)
    });
    document.getElementById('priceSum').innerText = `${priceSum}$`;

    $('.quantity button').on('click', function () {
        var button = $(this);
        var oldValue = button.parent().parent().find('input').val();
        if (button.hasClass('btn-plus')) {
            var newVal = parseFloat(oldValue) + 1;
        } else {
            if (oldValue > 0) {
                var newVal = parseFloat(oldValue) - 1;
            } else {
                newVal = 0;
            }
        }
        button.parent().parent().find('input').val(newVal);
        calculateTotal(button.parent().parent().find('input')[0].id);
    });
}

function calculateTotal(id)
{
    let prodId = id.split('_')[1];
    let quantity = parseInt(document.getElementById(`quant_${prodId}`).value);
    let price = parseInt(document.getElementById(`price_${prodId}`).innerText);
    let total = quantity * price;
    document.getElementById(`total_${prodId}`).innerText = `${total}$`;
    renderTotal();
}

function renderTotal() {
    let priceSum = 0;
    document.querySelectorAll('.totalPrice').forEach(tp => {
        priceSum += parseFloat(tp.innerText)
    });
    document.getElementById('priceSum').innerText = `${priceSum}$`;
}
function proceedCheckout() {
    let productDatas = document.querySelectorAll('.product-data');
    let products = [];
    productDatas.forEach((pd) => {
        let pid = pd.id.split('_')[1];
        products.push({
            productId: pid,
            quantity: parseInt(document.getElementById(`quant_${pid}`).value),
            price: parseInt(document.getElementById(`price_${pid}`).innerText),
            totalPrice: parseInt(document.getElementById(`total_${pid}`).innerText),
            name: document.getElementById(`name_${pid}`).innerText,
            photo: document.getElementById(`photo_${pid}`).src
        });
    });
    sendData(products);
}