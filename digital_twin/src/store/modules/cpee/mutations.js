export default {
    setUser(state, payload) {
        state.user = payload.email;
        state.coachID = payload.coachID;
    },
    logout(state) {
        state.user = null;
    },
    setCoach(state, id) {
        state.coachID = id;
    }
}