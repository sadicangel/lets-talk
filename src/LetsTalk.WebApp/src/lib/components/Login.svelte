<script lang="ts">
	let errors: { form?: string; username?: string; password?: string } = $state({});

	const login = async (event: Event) => {
		event.preventDefault();
		const username = (document.getElementById('username') as HTMLInputElement).value;
		const password = (document.getElementById('password') as HTMLInputElement).value;

		errors = {};

		if (!username) {
			errors.username = 'Username is required';
		} else if (!/^[a-zA-Z_][a-zA-Z0-9_]*$/.test(username)) {
			errors.username =
				'Username must contain only alphanumeric characters and underscores, and start with a letter or underscore';
		}

		if (!password) {
			errors.password = 'Password is required';
		}

		if (errors.username || errors.password) {
			return;
		}

		const response = await fetch('/api/auth/login', {
			method: 'POST',
			headers: {
				'Content-Type': 'application/x-www-form-urlencoded'
			},
			body: new URLSearchParams({
				username,
				password
			})
		});

		if (!response.ok) {
			switch (response.status) {
				case 401:
					errors.form = 'Invalid username or password.';
					break;
				default:
					errors.form = response.statusText;
					break;
			}
			return;
		}
	};
</script>

<div class="hero bg-base-200 min-h-screen">
	<div class="hero-content flex-col lg:flex-row-reverse">
		<div class="text-center lg:text-left">
			<h1 class="text-5xl font-bold">Login now!</h1>
			<p class="py-6">
				Welcome to Let's Talk, a chat application built with ASP.NET Core and Svelte. Connect with
				your friends and colleagues in real-time. Please log in to continue.
			</p>
		</div>
		<div class="card bg-base-100 w-full max-w-sm shrink-0 shadow-2xl">
			<form class="card-body" onsubmit={login}>
				{#if errors.form}
					<div class="form-control mb-4">
						<span class="text-error">{errors.form}</span>
					</div>
				{/if}
				<div class="form-control">
					<label class="label" for="username">
						<span class="label-text">Username</span>
					</label>
					<input
						id="username"
						type="text"
						placeholder="username"
						class="input input-bordered"
						required
					/>
					{#if errors.username}
						<span class="text-error">{errors.username}</span>
					{/if}
				</div>
				<div class="form-control">
					<label class="label" for="password">
						<span class="label-text">Password</span>
					</label>
					<input
						id="password"
						type="password"
						placeholder="password"
						class="input input-bordered"
						required
					/>
					{#if errors.password}
						<span class="text-error">{errors.password}</span>
					{/if}
					<p class="label">
						<a href="https://google.com" class="label-text-alt link link-hover">Forgot password?</a>
					</p>
				</div>
				<div class="form-control mt-6">
					<button class="btn btn-primary" type="submit">Login</button>
				</div>
			</form>
		</div>
	</div>
</div>
