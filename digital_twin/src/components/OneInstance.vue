<template>
    <div class="heading">
        <div class="tex">
            <h3 @click="toggleInstance">Instanz {{ instanceId }}</h3>
        </div>
        <div class="control">
            <span @click="$emit('delete-instance', instanceId)">❌</span>
        </div>
        <p class="show" @click="toggleInstance">{{ opened ? "⬆️" : "⬇️" }}</p>
    </div>
    <iframe v-if="opened" :src="instance" frameborder="0"></iframe>
</template>

<script>
export default {
    async created() {
      const formData = new URLSearchParams();
      formData.append('info', 'WfMS for the Digital Twin');
        const response = await fetch('http://localhost:8298/', {
            method: 'POST',
            headers: {
              'Content-Type': 'application/x-www-form-urlencoded',
            },
            body: formData
        });
        const instance = await response.json();
        this.instance = "http://localhost:8081/?monitor=http://localhost:8298/" + instance + "/";
    },
    props: [
        'instanceId'
    ],
    emits: [
        'delete-instance'
    ],
    data() {
        return {
            opened: true,
            instance: null
        }
    },
    methods: {
        toggleInstance() {
            this.opened = !this.opened;
        }
    }
}
</script>

<style scoped>
h3,
span,
p {
    cursor: pointer;
    user-select: none;
}

.heading {
    display: flex;
    justify-content: space-between;
    align-items: center;
    position: relative;
}

.show {
    position: absolute;
    top: calc(100% - 5px);
    left: 50%;
}

iframe {
    width: 100%;
    height: 100vh;
}
</style>