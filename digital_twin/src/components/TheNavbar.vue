<template>
    <div class="container">
        <h3><router-link to="/">Digitaler Zwilling Web Anwendung</router-link></h3>
        <ul>
            <li><router-link to="/form">Simulation erstellen</router-link></li>
            <li><router-link to="/wfms">WfMS</router-link></li>
            <li><router-link to="/dashboard">Dashboard</router-link></li>
            <li><router-link to="/worklist">Worklist</router-link></li>
        </ul>
    </div>
</template>

<style scoped>
.container {
    padding: 1rem 0.5rem;
    max-width: 1500px;
    margin: 0 auto;
    display: flex;
    justify-content: space-between;
    align-items: center;
}

ul {
    list-style: none;
    display: flex;
    justify-content: space-between;
    gap: 0 3.5rem;
}

a {
    text-decoration: none;
    color: #000;
    transition: all .3s;
}

a:hover,
.active {
    text-shadow: 0px 0px 1px blue;
}

@media only screen and (min-width: 901px) {
    .container {
        border-bottom: .5px solid #333;
    }
}

@media only screen and (max-width: 900px) {
  .container,
  ul {
    flex-direction: column;
    align-items: center;
    width: 100%;
  }

  ul li {
    border-bottom: 1px dotted #333;
    width: 100%;
    align-items: center;
    display: flex;
    flex-direction: column;
  }

  ul li:first-child {
    border-top: 1px dotted #333;
  }

  a {
    text-align: center;
  }
}
</style>