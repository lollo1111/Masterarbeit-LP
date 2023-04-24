<template>
    <the-card>
        <template #header>Workflow Management System</template>
        <template #default>
            <a @click="createInstance">Instanz erstellen</a>
            <p>
                Lorem, ipsum dolor sit amet consectetur adipisicing elit. Repellendus autem provident ipsam et inventore
                alias necessitatibus beatae perferendis, ratione placeat.
            </p>
            <ul>
                <keep-alive>
                    <li v-for="instance in instances">
                        <one-instance :instanceId="instance.id" @delete-instance="deleteInstance"
                            :key="instance.id"></one-instance>
                    </li>
                </keep-alive>
            </ul>
        </template>
    </the-card>
</template>

<script>
import OneInstance from '../components/OneInstance.vue';

export default {
    components: {
        OneInstance
    },
    data() {
        return {
            instances: [],
            currentCounter: 0
        }
    },
    methods: {
        createInstance() {
            const id = ++this.currentCounter;
            this.instances.push({
                id: id
            })
        },
        deleteInstance(instance) {
            this.instances = this.instances.filter(oneInstance => oneInstance.id !== instance);
        }
    }
}
</script>

<style scoped>
a {
    cursor: pointer;
    font-weight: bold;
}

p {
    margin-bottom: 2rem;
}

ul {
    list-style: none;
}

li {
    padding: 1rem;
    border-bottom: 1px dotted #333;
}

li:first-child {
    border-top: 1px dotted #333;
}
</style>