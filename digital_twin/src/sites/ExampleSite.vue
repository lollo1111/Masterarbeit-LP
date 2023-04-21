<template>
    <other-component></other-component>
    <the-dashboard></the-dashboard>
</template>

<script>
import TheDashboard from '../components/TheDashboard.vue';
import OtherComponent from '../components/OtherComponent.vue'

export default {
    components: {
        OtherComponent,
        TheDashboard
    }
}
</script>