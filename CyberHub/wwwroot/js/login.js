        // Create animated particles
    function createParticles() {
            const particles = document.querySelector('.particles');
    for (let i = 0; i < 50; i++) {
                const particle = document.createElement('div');
    particle.className = 'particle';
    particle.style.left = Math.random() * 100 + '%';
    particle.style.animationDelay = Math.random() * 20 + 's';
    particle.style.animationDuration = (Math.random() * 20 + 10) + 's';
    particles.appendChild(particle);
            }
        }

    // Password toggle functionality
    document.querySelector('.password-toggle').addEventListener('click', function() {
            const passwordInput = document.querySelector('input[type="password"]');
    const type = passwordInput.type === 'password' ? 'text' : 'password';
    passwordInput.type = type;
    this.textContent = type === 'password' ? '👁️' : '🙈';
        });

    // Form submission
    document.querySelector('form').addEventListener('submit', function(e) {
        e.preventDefault();
    const btn = document.querySelector('.login-btn');
    btn.textContent = 'Authenticating...';
    btn.style.background = 'linear-gradient(45deg, #4ecdc4, #45b7d1)';
            
            setTimeout(() => {
        btn.textContent = 'Access Granted ✓';
    btn.style.background = 'linear-gradient(45deg, #27ae60, #2ecc71)';
            }, 2000);
        });

    // Initialize particles
    createParticles();
