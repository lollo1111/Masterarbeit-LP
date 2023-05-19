<template>
    <the-card>
        <template #header>Dashboard</template>
        <template #default>
            <iframe src="http://localhost:3000/d/q9FIABE4k/digital-twin-dashboard?orgId=1&refresh=1s" frameborder="0"></iframe>
        </template>
    </the-card>
</template>

<style scoped>
iframe {
    width: 100%;
    height: 100vh;
}
</style>