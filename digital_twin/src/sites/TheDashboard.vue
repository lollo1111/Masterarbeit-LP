<template>
    <the-card>
        <template #header>Dashboard</template>
        <template #default>
            <p>
                Lorem, ipsum dolor sit amet consectetur adipisicing elit. Repellendus autem provident ipsam et inventore alias necessitatibus beatae perferendis, ratione placeat.
            </p>
            <iframe src="http://localhost:3000/d/q9FIABE4k/digital-twin-dashboard?orgId=1&kiosk=tv&refresh=5s" frameborder="0"></iframe>
        </template>
    </the-card>
</template>

<style scoped>
iframe {
    width: 100%;
    height: 100vh;
}
</style>