// Scripts globais da interface.

// Esconde o "Ler completo" quando o texto do versículo já cabe na faixa.
(function () {
    const previa = document.querySelector('[data-verso-preview]');
    const botao = document.querySelector('[data-verso-botao]');
    if (!previa || !botao) {
        return;
    }

    function ajustar() {
        botao.hidden = previa.scrollHeight <= previa.clientHeight + 1;
    }

    ajustar();
    window.addEventListener('resize', ajustar);
})();

// Rolagem suave para os links internos da página.
(function () {
    const links = document.querySelectorAll('a[href^="#"]');

    function animar(destino) {
        const inicio = window.scrollY;
        const distancia = destino - inicio;
        const duracao = 550;
        const t0 = performance.now();

        function passo(agora) {
            const t = Math.min((agora - t0) / duracao, 1);
            const suave = 1 - Math.pow(1 - t, 3);
            window.scrollTo(0, inicio + distancia * suave);
            if (t < 1) {
                requestAnimationFrame(passo);
            }
        }

        requestAnimationFrame(passo);
    }

    links.forEach(function (link) {
        link.addEventListener('click', function (evento) {
            const href = link.getAttribute('href');
            if (!href || href === '#') {
                return;
            }

            const alvo = document.querySelector(href);
            if (!alvo) {
                return;
            }

            evento.preventDefault();
            const margem = 88;
            const topo = alvo.getBoundingClientRect().top + window.scrollY - margem;
            animar(Math.max(0, topo));
        });
    });
})();
