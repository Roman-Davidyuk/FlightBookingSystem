import http from 'k6/http';
import { check, sleep } from 'k6';

export const options = {
    stages: [
        { duration: '2m', target: 150 }, // Швидкий розгін до 150 користувачів
        { duration: '5m', target: 150 }, // Тримаємо високе навантаження 5 хвилин
        { duration: '2m', target: 0 },   // Охолодження
    ],
    thresholds: {
        http_req_duration: ['p(90)<1000'], // При стресі допускаємо затримки до 1 сек
    },
};

const BASE_URL = 'http://localhost:5063';

export default function () {
    let res = http.get(`${BASE_URL}/api/flights?origin=Odesa`);
    check(res, { 'search alive': (r) => r.status === 200 });
    
    const payload = JSON.stringify({
        flightId: '144e325c-6a09-43f9-879b-b0242fd1f844',
        passengerId: '0e5a7f3c-71df-4cd1-948f-1d1b862f88ff',
        seatNumber: `ST-${Math.floor(Math.random() * 100000)}`
    });
    http.post(`${BASE_URL}/api/bookings`, payload, { headers: { 'Content-Type': 'application/json' } });
    sleep(1);
}