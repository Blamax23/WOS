document.addEventListener('DOMContentLoaded', () => {
    const logoWos = document.querySelector('.logoWos');
    const elements = logoWos.querySelectorAll('.logoPrincipal');

    logoWos.addEventListener('mousemove', (e) => {
        const { width, height, left, top } = logoWos.getBoundingClientRect();
        const centerX = width / 2;
        const centerY = height / 2;
        const mouseX = e.clientX - left;
        const mouseY = e.clientY - top;

        // Calcul du déplacement
        const moveX = (mouseX - centerX) / centerX * 30;
        const moveY = (mouseY - centerY) / centerY * 30;

        elements.forEach((el, index) => {
            // Effets différents selon la position
            const multiplier = index === 0 ? -1 : (index === 2 ? 1 : 0);

            el.style.transform = `translate(${moveX * multiplier}px, ${moveY * multiplier}px)`;
            el.style.transition = 'transform 0.1s ease-out';
        });
    });

    // Réinitialisation au départ de la souris
    logoWos.addEventListener('mouseleave', () => {
        elements.forEach(el => {
            el.style.transform = 'translate(0, 0)';
        });
    });
});