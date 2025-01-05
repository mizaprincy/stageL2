import { reactive } from 'vue';

export const eventBus = reactive({
  events: new Map<string, Function[]>(),
  emit(event: string, ...args: any[]) {
    this.events.get(event)?.forEach(callback => callback(...args));
  },
  on(event: string, callback: Function) {
    if (!this.events.has(event)) {
      this.events.set(event, []);
    }
    this.events.get(event)?.push(callback);
  },
  off(event: string, callback: Function) {
    const callbacks = this.events.get(event);
    if (callbacks) {
      this.events.set(event, callbacks.filter(cb => cb !== callback));
    }
  },
});
