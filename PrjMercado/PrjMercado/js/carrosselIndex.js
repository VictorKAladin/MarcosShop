(function () {
    'use strict';

    function iniciarCarrossel() {
        var carrossel = document.getElementById('carrosselDestaques');
        if (!carrossel) return;

        var slides = carrossel.querySelectorAll('.carrossel-slide');
        var indicadores = carrossel.querySelectorAll('.indicador[data-slide]');
        var anterior = carrossel.querySelector('[data-direcao="anterior"]');
        var proximo = carrossel.querySelector('[data-direcao="proximo"]');
        var indiceAtual = 0;
        var temporizador = null;
        var mouseSobre = false;
        var focoInterno = false;

        if (!slides.length) return;

        function mostrarSlide(indice) {
            indiceAtual = (indice + slides.length) % slides.length;

            for (var i = 0; i < slides.length; i++) {
                var ativo = i === indiceAtual;
                if (ativo) slides[i].classList.add('ativo');
                else slides[i].classList.remove('ativo');
                slides[i].setAttribute('aria-hidden', ativo ? 'false' : 'true');
            }

            for (var j = 0; j < indicadores.length; j++) {
                var indicadorAtivo = j === indiceAtual;
                if (indicadorAtivo) indicadores[j].classList.add('ativo');
                else indicadores[j].classList.remove('ativo');
                indicadores[j].setAttribute('aria-pressed', indicadorAtivo ? 'true' : 'false');
                if (indicadorAtivo) indicadores[j].setAttribute('aria-current', 'true');
                else indicadores[j].removeAttribute('aria-current');
            }
        }

        function pararTemporizador() {
            if (temporizador !== null) {
                window.clearInterval(temporizador);
                temporizador = null;
            }
        }

        function iniciarTemporizador() {
            pararTemporizador();
            if (mouseSobre || focoInterno || document.hidden) return;
            temporizador = window.setInterval(function () {
                mostrarSlide(indiceAtual + 1);
            }, 5000);
        }

        if (anterior) {
            anterior.addEventListener('click', function () {
                mostrarSlide(indiceAtual - 1);
                iniciarTemporizador();
            });
        }

        if (proximo) {
            proximo.addEventListener('click', function () {
                mostrarSlide(indiceAtual + 1);
                iniciarTemporizador();
            });
        }

        for (var k = 0; k < indicadores.length; k++) {
            indicadores[k].addEventListener('click', function () {
                    mostrarSlide(parseInt(this.getAttribute('data-slide'), 10));
                    iniciarTemporizador();
            });
        }

        carrossel.addEventListener('mouseenter', function () {
            mouseSobre = true;
            pararTemporizador();
        });

        carrossel.addEventListener('mouseleave', function () {
            mouseSobre = false;
            iniciarTemporizador();
        });

        carrossel.addEventListener('focusin', function () {
            focoInterno = true;
            pararTemporizador();
        });

        carrossel.addEventListener('focusout', function (evento) {
            if (!carrossel.contains(evento.relatedTarget)) {
                focoInterno = false;
                iniciarTemporizador();
            }
        });

        carrossel.addEventListener('keydown', function (evento) {
            if (evento.key === 'ArrowLeft') {
                mostrarSlide(indiceAtual - 1);
                iniciarTemporizador();
            } else if (evento.key === 'ArrowRight') {
                mostrarSlide(indiceAtual + 1);
                iniciarTemporizador();
            }
        });

        document.addEventListener('visibilitychange', iniciarTemporizador);
        window.addEventListener('pagehide', pararTemporizador);

        mostrarSlide(0);
        iniciarTemporizador();
    }

    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', iniciarCarrossel);
    } else {
        iniciarCarrossel();
    }
}());
