var counter = 0;

function fnLoadContent() {

    var payMethod = $('#slPaymentMethods').val();
    var bankAcc = $('#slBankAccounts').val();
    var cusId = document.getElementsByName('cbbCustomers')[0].value;
    var from = $('#dtFrom').val();
    var to = $('#dtTo').val();

    LoadContentDynamic("CashBalancesUC", "Accounting/CashBalancesUC", {
        payMethod: payMethod,
        bankAcc: bankAcc,
        from: from,
        to: to,
        partnerId: cusId
    });
    
}