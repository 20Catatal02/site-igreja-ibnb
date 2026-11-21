document.addEventListener("DOMContentLoaded", function () {
    const chkRecorrente = document.getElementById("chkRecorrente");
    const divCampos = document.getElementById("divCamposRecorrencia");
    const selTipo = document.getElementById("selTipoRecorrencia");
    const divPersonalizado = document.getElementById("divPersonalizado");
    const inputImagem = document.getElementById("inputImagem");
    const preview = document.getElementById("previewImagem");

    function atualizarRecorrencia() {
        if (divCampos && chkRecorrente) {
            divCampos.classList.toggle("d-none", !chkRecorrente.checked);
        }
        if (divPersonalizado && selTipo) {
            divPersonalizado.classList.toggle("d-none", selTipo.value !== "Personalizado");
        }
    }

    if (chkRecorrente) chkRecorrente.addEventListener("change", atualizarRecorrencia);
    if (selTipo) selTipo.addEventListener("change", atualizarRecorrencia);
    atualizarRecorrencia();

    if (inputImagem && preview) {
        inputImagem.addEventListener("change", function () {
            const arquivo = this.files && this.files[0];
            if (!arquivo) {
                preview.classList.add("d-none");
                return;
            }
            const leitor = new FileReader();
            leitor.onload = function (e) {
                preview.src = e.target.result;
                preview.classList.remove("d-none");
            };
            leitor.readAsDataURL(arquivo);
        });
    }
});
