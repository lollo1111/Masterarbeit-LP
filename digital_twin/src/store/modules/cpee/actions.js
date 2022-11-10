export default {
    async loadRequests(context) {
        const usedUrl = url+'/requests';
        const response = await fetch(usedUrl);
        const responseData = await response.json();

        if (!response.ok) {
            const error = new Error(responseData.message || 'Failed to send request!');
            throw error;
        }
        context.commit('loadRequests', responseData);
    }
};