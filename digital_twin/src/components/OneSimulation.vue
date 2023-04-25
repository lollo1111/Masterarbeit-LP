<template>
    <p><strong>ID</strong>: {{ simulation.id }}</p>
    <p><strong>JSON Details</strong>:</p>
    <div class="json">
        <p>{{ JSON.stringify(simulation.details) }}</p>
    </div>
</template>

<script>
export default {
    props: [
        'simulation'
    ]
}
</script>

<style scoped>
.json{
    padding: .5rem;
}
</style>