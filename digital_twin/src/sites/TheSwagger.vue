<template>
    <the-card>
        <template #header>Swagger</template>
        <template #default>
            <p>
                Lorem, ipsum dolor sit amet consectetur adipisicing elit. Repellendus autem provident ipsam et inventore alias necessitatibus beatae perferendis, ratione placeat.
            </p>
            <iframe src="http://localhost:9011" frameborder="0"></iframe>
        </template>
    </the-card>
</template>

<style scoped>
iframe {
    width: 100%;
    height: 100vh;
}
</style>