<template>
    <p><strong>Gerät</strong>: {{ device.name }}</p>
    <p><strong>Typ</strong>: {{ device.type }}</p>
    <p><strong>Wert</strong>: {{ device.value }}</p>
</template>

<script>
export default {
    props: [
        'device'
    ]
}
</script>