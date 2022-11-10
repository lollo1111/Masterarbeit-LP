<template>
    <div class="container">
        <iframe src="http://localhost:8081/?monitor=http://localhost:8298/1/" frameborder="0"></iframe>
    </div>
</template>

<style scoped>
.container {
    max-width: 1200px;
    height: 100vh;
    margin: 0 auto;
    padding: 0 2rem;
}

iframe {
    width: 100%;
    height: 100%;
}
</style>