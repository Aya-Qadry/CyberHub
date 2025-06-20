    function handleFormSubmit(event) {
        event.preventDefault();

    // Simple form validation
    const username = document.getElementById('username').value;
    const email = document.getElementById('email').value;
    const password = document.getElementById('new-password').value;

    let isValid = true;

            // Reset error states
            document.querySelectorAll('.form-control').forEach(el => {
        el.classList.remove('error', 'success');
            });
            document.querySelectorAll('.error-message').forEach(el => {
        el.style.display = 'none';
            });

    // Validate username
    if (username.trim() === '') {
        document.getElementById('username').classList.add('error');
    document.getElementById('username-error').style.display = 'block';
    isValid = false;
            } else {
        document.getElementById('username').classList.add('success');
            }

    // Validate email
    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    if (!emailRegex.test(email)) {
        document.getElementById('email').classList.add('error');
    document.getElementById('email-error').style.display = 'block';
    isValid = false;
            } else {
        document.getElementById('email').classList.add('success');
            }

    // Validate password if provided
    if (password && password.length < 8) {
        document.getElementById('new-password').classList.add('error');
    document.getElementById('password-error').style.display = 'block';
    isValid = false;
            } else if (password) {
        document.getElementById('new-password').classList.add('success');
            }

    if (isValid) {
                // Simulate saving
                const btn = document.querySelector('.post-btn');
    const originalText = btn.innerHTML;
    btn.innerHTML = '<i class="fas fa-spinner fa-spin"></i> Saving...';
    btn.disabled = true;
                
                setTimeout(() => {
        btn.innerHTML = '<i class="fas fa-check"></i> Saved!';
                    setTimeout(() => {
        btn.innerHTML = originalText;
    btn.disabled = false;
                    }, 2000);
                }, 1500);
            }
        }

        // Add focus/blur effects to form inputs
        document.querySelectorAll('.form-control').forEach(input => {
        input.addEventListener('focus', function () {
            this.parentElement.classList.add('focused');
        });

    input.addEventListener('blur', function() {
        this.parentElement.classList.remove('focused');
            });
        });

