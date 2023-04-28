<template>
    <the-card>
        <template #header>Workflow Management System</template>
        <template #default>
            <div @click="createInstance" class="create">
                <a>🆕<span class="textspan">Instanz erstellen</span></a>
                <p>
                    Lorem, ipsum dolor sit amet consectetur adipisicing elit. Repellendus autem provident ipsam et inventore
                    alias necessitatibus beatae perferendis, ratione placeat.
                </p>
            </div>
            <ul>
                <keep-alive>
                    <li v-for="instance in instances" :key="instance.id">
                        <one-instance :instanceId="instance.id" @delete-instance="deleteInstance"></one-instance>
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
.create {
    border: 1px solid #000;
    border-radius: 5px;
    padding: 1.5rem;
    margin-bottom: 2rem;
    cursor: pointer;
}

.create:hover {
    border: 2px solid #000;
    background-color: #ccc;
}

a {
    font-weight: bold;
}

ul {
    list-style: none;
}

li {
    padding: 1rem 1rem 1rem 0;
    border-bottom: 1px dotted #333;
}

li:first-child {
    border-top: 1px dotted #333;
}

.create:hover .textspan {
    text-decoration: underline 1px solid #000;
}
</style>