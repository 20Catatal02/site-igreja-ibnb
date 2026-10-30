// Variável global para instanciar o modal do Bootstrap
let modalInstance = null;

// Abre o Modal (se id for nulo, é Create; se tiver id, é Edit)
async function abrirModal(id = null) {
    const url = id ? `/Aniversariantes/FormModal?id=${id}` : '/Aniversariantes/FormModal';
    
    try {
        const response = await fetch(url);
        const html = await response.text();
        
        document.getElementById('modalContent').innerHTML = html;
        
        if (!modalInstance) {
            modalInstance = new bootstrap.Modal(document.getElementById('modalAniversariante'));
        }
        modalInstance.show();

        // Intercepta o submit do formulário para enviar via AJAX
        document.getElementById('formAniversariante').addEventListener('submit', salvar);
    } catch (error) {
        console.error('Erro ao carregar o modal:', error);
    }
}

// Salva os dados (Create/Edit)
async function salvar(e) {
    e.preventDefault();
    const form = e.target;
    const formData = new FormData(form);

    try {
        const response = await fetch(form.action, {
            method: 'POST',
            body: formData
        });

        if (response.ok) {
            // Se retornou 200 OK, a validação passou. Fecha o modal e recarrega a página.
            modalInstance.hide();
            window.location.reload(); 
        } else {
            // Se retornou erro (ex: validação falhou), injeta o HTML com os erros novamente
            const html = await response.text();
            document.getElementById('modalContent').innerHTML = html;
            document.getElementById('formAniversariante').addEventListener('submit', salvar);
        }
    } catch (error) {
        console.error('Erro ao salvar:', error);
    }
}

// Exclui o registro
async function excluir(id, nome) {
    if (!confirm(`Tem certeza que deseja excluir o aniversariante "${nome}"?`)) {
        return;
    }

    // Pega o token de segurança gerado pelo @Html.AntiForgeryToken()
    const token = document.querySelector('input[name="__RequestVerificationToken"]').value;

    try {
        const response = await fetch(`/Aniversariantes/Excluir/${id}`, {
            method: 'POST',
            headers: {
                'RequestVerificationToken': token
            }
        });

        if (response.ok) {
            window.location.reload();
        } else {
            alert('Erro ao tentar excluir o aniversariante.');
        }
    } catch (error) {
        console.error('Erro de rede:', error);
    }
}