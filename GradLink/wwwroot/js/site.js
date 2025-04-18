document.addEventListener('DOMContentLoaded', function () {
    const passwordInput = document.getElementById('Password');
    const confirmPasswordInput = document.getElementById('ConfirmPassword');
    const passwordRequirements = document.getElementById('passwordRequirements');
    const confirmPasswordHelp = document.getElementById('confirmPasswordHelp');

    const lengthRequirement = document.getElementById('lengthRequirement');
    const uppercaseRequirement = document.getElementById('uppercaseRequirement');
    const lowercaseRequirement = document.getElementById('lowercaseRequirement');
    const numberRequirement = document.getElementById('numberRequirement');

    function validatePasswordStrength(password) {
        const hasUppercase = /[A-Z]/.test(password);
        const hasLowercase = /[a-z]/.test(password);
        const hasNumber = /\d/.test(password);
        const isLongEnough = password.length >= 6;

        return {
            hasUppercase,
            hasLowercase,
            hasNumber,
            isLongEnough
        };
    }

    function updateRequirements(password) {
        const strength = validatePasswordStrength(password);

        if (strength.isLongEnough) {
            lengthRequirement.classList.add('valid');
            lengthRequirement.classList.remove('invalid');
            lengthRequirement.classList.add('hidden');
        } else {
            lengthRequirement.classList.add('invalid');
            lengthRequirement.classList.remove('valid');
            lengthRequirement.classList.remove('hidden');
        }

        if (strength.hasUppercase) {
            uppercaseRequirement.classList.add('valid');
            uppercaseRequirement.classList.remove('invalid');
            uppercaseRequirement.classList.add('hidden');
        } else {
            uppercaseRequirement.classList.add('invalid');
            uppercaseRequirement.classList.remove('valid');
            uppercaseRequirement.classList.remove('hidden');
        }

        if (strength.hasLowercase) {
            lowercaseRequirement.classList.add('valid');
            lowercaseRequirement.classList.remove('invalid');
            lowercaseRequirement.classList.add('hidden');
        } else {
            lowercaseRequirement.classList.add('invalid');
            lowercaseRequirement.classList.remove('valid');
            lowercaseRequirement.classList.remove('hidden');
        }

        if (strength.hasNumber) {
            numberRequirement.classList.add('valid');
            numberRequirement.classList.remove('invalid');
            numberRequirement.classList.add('hidden');
        } else {
            numberRequirement.classList.add('invalid');
            numberRequirement.classList.remove('valid');
            numberRequirement.classList.remove('hidden');
        }
    }

    function validateConfirmPassword() {
        const password = passwordInput.value;
        const confirmPassword = confirmPasswordInput.value;

        if (confirmPassword && password !== confirmPassword) {
            confirmPasswordHelp.textContent = "Passwords do not match.";
            confirmPasswordHelp.classList.remove('d-none');
            confirmPasswordInput.classList.add('invalid');
            confirmPasswordInput.classList.remove('valid');
        } else {
            confirmPasswordHelp.classList.add('d-none');
            confirmPasswordInput.classList.add('valid');
            confirmPasswordInput.classList.remove('invalid');
        }
    }

    passwordInput.addEventListener('focus', function () {
        passwordRequirements.classList.remove('hidden');
    });

    passwordInput.addEventListener('blur', function () {
        if (!passwordInput.value) {
            passwordRequirements.classList.add('hidden');
        }
    });

    passwordInput.addEventListener('input', function () {
        updateRequirements(passwordInput.value);
    });

    confirmPasswordInput.addEventListener('input', validateConfirmPassword);

    togglePassword.addEventListener('click', function () {
        const type = passwordInput.type === 'password' ? 'text' : 'password';
        passwordInput.type = type;

        togglePassword.querySelector('i').classList.toggle('fa-eye');
        togglePassword.querySelector('i').classList.toggle('fa-eye-slash');
    });

    toggleConfirmPassword.addEventListener('click', function () {
        const type = confirmPasswordInput.type === 'password' ? 'text' : 'password';
        confirmPasswordInput.type = type;

        toggleConfirmPassword.querySelector('i').classList.toggle('fa-eye');
        toggleConfirmPassword.querySelector('i').classList.toggle('fa-eye-slash');
    });
});

document.addEventListener("DOMContentLoaded", function () {
    const menuItems = document.querySelectorAll('.menu-item');

    menuItems.forEach(item => {
        item.addEventListener('click', function () {
            menuItems.forEach(menuItem => menuItem.classList.remove('active'));
            this.classList.add('active');
        });
    });
});

const darkModeToggle = document.getElementById('darkModeToggle');
const body = document.body;

const savedDarkMode = localStorage.getItem('dark-mode');

if (savedDarkMode === 'enabled') {
    body.classList.add('dark-mode');
    darkModeToggle.textContent = '☀️';
} else {
    darkModeToggle.textContent = '🌙';
}

darkModeToggle.addEventListener('click', () => {
    if (body.classList.contains('dark-mode')) {
        body.classList.remove('dark-mode');
        darkModeToggle.textContent = '🌙';
        localStorage.setItem('dark-mode', 'disabled');
    } else {
        body.classList.add('dark-mode');
        darkModeToggle.textContent = '☀️';
        localStorage.setItem('dark-mode', 'enabled');
    }
});

const notificationBell = document.getElementById('notificationBell');
const notificationDropdown = document.getElementById('notificationDropdown');

notificationBell.addEventListener('click', (e) => {
    e.stopPropagation();

    const isVisible = notificationDropdown.style.display === 'block';
    notificationDropdown.style.display = isVisible ? 'none' : 'block';
});

document.addEventListener('click', () => {
    notificationDropdown.style.display = 'none';
});

const postInputContainer = document.getElementById('postInputContainer');
const postModal = document.getElementById('postModal');
const closeModal = document.getElementById('closeModal');
const postButton = document.getElementById('postButton');

postInputContainer.addEventListener('click', () => {
    debugger
    postModal.style.display = "block";
});

closeModal.addEventListener('click', () => {
    postModal.style.display = "none";
});

window.addEventListener('click', (event) => {
    if (event.target === postModal) {
        postModal.style.display = "none";
    }
});

postButton.addEventListener('click', () => {
    const postContent = document.getElementById('postTextArea').value;
    const file = document.getElementById('fileUpload').files[0];

    if (postContent.trim() || file) {
        postModal.style.display = "none";
        document.getElementById('postTextArea').value = "";
        document.getElementById('fileUpload').value = "";
    } else {
        alert("Please write something or upload a file.");
    }
});


