const addBtn = document.getElementById('add-to-cart-btn');

addBtn.addEventListener('click', (ev) => {
    ev.preventDefault();
    const pid = addBtn.dataset['pid'];
    const quantity = document.getElementById('product-quantity').value;
    let products = localStorage.getItem('cartProducts');
    let product = {
        id: pid,
        quantity: quantity
    };

    if (!products) {  
        localStorage.setItem('cartProducts', JSON.stringify([product]));
    } else {
        let prodArr = Array.from(JSON.parse(products));
        let foundPrd = prodArr.findIndex(prd => prd.id == product.id);
        if (foundPrd == -1) {
            prodArr.push(product);
            localStorage.setItem('cartProducts', JSON.stringify(prodArr));
        }
        else {
            prodArr.splice(foundPrd, 1);
            prodArr.push(product);
            localStorage.setItem('cartProducts', JSON.stringify(prodArr));
        }
    }
});