import axios, { type AxiosInstance } from 'axios'
import router from '@/router';

// Créer une instance Axios
const backendUrl = import.meta.env.VITE_BACKEND_URL
const axiosInstance: AxiosInstance = axios.create({
    baseURL: `${backendUrl}`, // L'URL de votre API
    headers: {
        'Content-Type': 'application/json',
    },
});

// Intercepteur pour ajouter le token dans les requêtes
axiosInstance.interceptors.request.use(
    (config) => {
        const token = sessionStorage.getItem('token'); // Récupérer le token depuis sessionStorage
        if (token) {
            config.headers!['Authorization'] = `Bearer ${token}`; // Ajouter le token dans les en-têtes
        }
        return config;
    },
    (error) => {
        return Promise.reject(error);
    }
);

// Intercepteur pour gérer les erreurs de réponse
axiosInstance.interceptors.response.use(
    (response) => response,
    (error) => {
        if (error.response && (error.response.status === 401 || error.response.status === 403)) {
            console.warn('Unauthorized or Forbidden! Redirecting to login...');
            router.push('/') // Rediriger l'utilisateur vers la page de login
        }
        return Promise.reject(error);
    }
);

export default axiosInstance;
