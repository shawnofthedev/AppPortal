// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
var checker = document.getElementById('requestForm_ReplaceAsset');
var assetText = document.getElementById('requestForm_AssetNum');
var assetLabel = document.getElementById('requestForm_lblAssetNum');

if (checker.checked) {
    assetText.style.visibility = 'visible';
    assetLabel.style.visibility = 'visible';
} else {
    assetText.style.visibility = 'hidden'; 
    assetLabel.style.visibility = 'hidden'; 
}
 
checker.onchange = function () {
    if (checker.checked) {
        assetText.style.visibility = 'visible';
        assetLabel.style.visibility = 'visible';
    } else {
        assetText.style.visibility = 'hidden'; 
        assetLabel.style.visibility = 'hidden'; 
        assetText.value = '';
    }
};

