<template>
    <p><strong>ID</strong>: {{ simulation.id }}</p>
    <p><strong>JSON</strong>:</p>
    <div class="json">
        <p>{{ JSON.stringify(simulation) }}</p>
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