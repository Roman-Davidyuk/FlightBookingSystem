import http from 'k6/http';
import { check, sleep } from 'k6';

export const options = {
    vus: 1, 
    duration: '30s', 
    thresholds: {
        http_req_failed: ['rate==0.00'], 
        http_req_duration: ['p(95)<300'],
    },
};

const BASE_URL = 'http://localhost:5063';

export default function () {
    let res = http.get(`${BASE_URL}/api/flights?origin=Kyiv`);
    check(res, { 'search is 200': (r) => r.status === 200 });
    sleep(1);
    
    const payload = JSON.stringify({
        flightId: '144e325c-6a09-43f9-879b-b0242fd1f844',
        passengerId: '0e5a7f3c-71df-4cd1-948f-1d1b862f88ff',
        seatNumber: `SM-${Math.floor(Math.random() * 1000)}`
    });
    res = http.post(`${BASE_URL}/api/bookings`, payload, { headers: { 'Content-Type': 'application/json' } });
    check(res, { 'booking is 201 or 400': (r) => [201, 400].includes(r.status) });
    sleep(1);
}