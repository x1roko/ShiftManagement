(function () {
	let THEME_KEY = 'site_theme';
	let body = document.body;
	let toggleBtn = null;
	let icon = null;

	function applyTheme(theme) {
		if (theme === 'dark') {
			body.classList.add('dark-theme');
			if (icon) {
				icon.classList.remove('bi-moon-fill');
				icon.classList.add('bi-sun-fill');
			}
			if (toggleBtn) toggleBtn.setAttribute('aria-pressed', 'true');
		} else {
			body.classList.remove('dark-theme');
			if (icon) {
				icon.classList.remove('bi-sun-fill');
				icon.classList.add('bi-moon-fill');
			}
			if (toggleBtn) toggleBtn.setAttribute('aria-pressed', 'false');
		}
	}

	function getPreferredTheme() {
		const saved = localStorage.getItem(THEME_KEY);
		if (saved === 'dark' || saved === 'light') return saved;
		if (window.matchMedia && window.matchMedia('(prefers-color-scheme: dark)').matches) return 'dark';
		return 'light';
	}

	function toggleTheme() {
		const current = body.classList.contains('dark-theme') ? 'dark' : 'light';
		const next = current === 'dark' ? 'light' : 'dark';
		applyTheme(next);
		localStorage.setItem(THEME_KEY, next);
	}

	window.addEventListener('load', () => {
		// query elements after DOM is ready
		toggleBtn = document.getElementById('themeToggle');
		icon = document.getElementById('themeIcon');

		// apply saved or preferred theme
		try {
			applyTheme(getPreferredTheme());
		} catch (e) {
			// fail silently but log for debugging
			console.error('Theme apply error', e);
		}

		if (toggleBtn) {
			toggleBtn.addEventListener('click', (e) => {
				e.preventDefault();
				toggleTheme();
			});
		}
	});
})();
