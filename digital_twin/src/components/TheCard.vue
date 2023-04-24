<template>
    <div class="container">
        <h1><slot name="header"></slot></h1>
        <div class="line"></div>
        <slot></slot>
    </div>
</template>

<style scoped>
.container {
    max-width: 1500px;
    margin: 3rem auto;
    padding: .5rem;
}

h1 {
    margin-bottom: 2rem;
}
</style>