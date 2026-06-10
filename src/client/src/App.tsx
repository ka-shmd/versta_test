import {AppLayout} from "@/components/layout/app-layout.tsx";
import {Routes, Route} from "react-router-dom";
import {CreateOrderPage} from "@/pages/create-order-page.tsx";
import {OrdersListPage} from "@/pages/orders-list-page.tsx";
import {OrderDetailPage} from "@/pages/order-detail-page.tsx";

export function App() {
  return (
    <Routes>
      <Route element={<AppLayout />}>
        <Route path="/" element={<CreateOrderPage />}/>
        <Route path="/orders" element={<OrdersListPage />}/>
        <Route path="/orders/:orddderNumber" element={<OrderDetailPage />}/>
      </Route>
    </Routes>
  )
}

export default App
