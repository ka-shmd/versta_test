import { Link, Outlet } from "react-router-dom"

export function AppLayout() {
  return (
    <div className="min-h-svh">
      <header className="border-b">
        <div className="mx-auto flex max-w-5xl items-center justify-between px-6 py-4">
          <span className="front-medium">
            Тест Версты
          </span>
          <nav className="flex gap-4 text-sm">
            <Link to="/">Новый заказ</Link>
            <Link to="/orders">Список заказов</Link>
          </nav>
        </div>
      </header>
      <main className="mx-auto max-w-5xl p-6">
        <Outlet />
      </main>
    </div>
  )
}
