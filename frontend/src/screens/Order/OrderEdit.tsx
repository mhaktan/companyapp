import React, { useState, useEffect } from 'react';
import { useMutation } from '@tanstack/react-query';
import { TkButton, TkDatepicker, TkInput, TkSelect } from '@takeoff-ui/react';
import { dataProvider } from '../../dataProvider';
import { overlayStyle, modalStyle } from '../../styles';
import { LookupSelect } from '../../shared/LookupSelect';
import { useFlows } from '../../flows/FlowProvider';

type OrderRecord = {
  id: string | number;
  orderNo: string;
  orderDate: string;
  deliveryDate?: string;
  discountRate?: number;
  vatRate?: number;
  subtotal?: number;
  discountAmount?: number;
  vatAmount?: number;
  grandTotal?: number;
  notes?: string;
  status: string;
  companyId: string;
};

interface OrderEditProps {
  record: OrderRecord | null;
  onClose: () => void;
  onSuccess: () => void;
}

export const OrderEdit: React.FC<OrderEditProps> = ({ record, onClose, onSuccess }) => {
  const [form, setForm] = useState<Partial<OrderRecord>>(record ?? {});
  const setField = (name: string, value: unknown) => setForm((p) => ({ ...p, [name]: value }));
  const { triggerFlows } = useFlows();

  useEffect(() => {
    if (record) setForm({ ...record });
  }, [record]);

  const mutation = useMutation({
    mutationFn: (values: Partial<OrderRecord>) =>
      dataProvider.update('Order', record!.id, values),
    onSuccess: (_data, values) => { triggerFlows('update', 'Order', values as Record<string, unknown>); onSuccess(); onClose(); },
    onError: (err: Error) => { window.dispatchEvent(new CustomEvent('app-toast', { detail: { type: 'error', message: err.message } })); },
  });

  if (!record) return null;

  return (
    <div style={overlayStyle} onClick={onClose}>
      <div style={modalStyle} onClick={(e) => e.stopPropagation()}>
        <div style={{ padding: '20px 28px 0', flexShrink: 0, display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
          <h2 style={{ margin: 0, fontSize: 18, fontWeight: 700 }}>Edit Order</h2>
          <button onClick={onClose} style={{ background: 'none', border: 'none', fontSize: 20, cursor: 'pointer', color: '#666', padding: '4px 8px', borderRadius: 4 }} onMouseOver={(e) => (e.currentTarget.style.color = '#333')} onMouseOut={(e) => (e.currentTarget.style.color = '#666')}>✕</button>
        </div>
        <form onSubmit={(e) => { e.preventDefault(); mutation.mutate(form); }} style={{ display: 'flex', flexDirection: 'column', flex: 1, overflow: 'hidden' }}>
          <div style={{ flex: 1, overflowY: 'auto', padding: '20px 28px' }}>
            <div style={{ display: 'grid', gridTemplateColumns: 'repeat(12, minmax(0, 1fr))', gap: '16px' }}>
                <div style={{ gridColumn: 'span 6' }}>
                  <TkInput mode="text" label="Order No *" value={String(form.orderNo ?? '')} onTkChange={(e: CustomEvent) => ((v) => setField('orderNo', v))(e.detail)} />
                </div>
                <div style={{ gridColumn: 'span 3' }}>
                  <TkDatepicker label="Order Date *" value={String(form.orderDate ?? '')} onTkChange={(e: CustomEvent) => ((v) => setField('orderDate', v))(e.detail)} />
                </div>
                <div style={{ gridColumn: 'span 3' }}>
                  <TkDatepicker label="Delivery Date" value={String(form.deliveryDate ?? '')} onTkChange={(e: CustomEvent) => ((v) => setField('deliveryDate', v))(e.detail)} />
                </div>
                <div style={{ gridColumn: 'span 6' }}>
                  <TkInput mode="number" label="Discount Rate" value={String(form.discountRate ?? '')} onTkChange={(e: CustomEvent) => ((v) => setField('discountRate', v === '' ? null : Number(v)))(e.detail)} />
                </div>
                <div style={{ gridColumn: 'span 6' }}>
                  <TkInput mode="number" label="Vat Rate" value={String(form.vatRate ?? '')} onTkChange={(e: CustomEvent) => ((v) => setField('vatRate', v === '' ? null : Number(v)))(e.detail)} />
                </div>
                <div style={{ gridColumn: 'span 6' }}>
                  <TkInput mode="number" label="Subtotal" value={String(form.subtotal ?? '')} onTkChange={(e: CustomEvent) => ((v) => setField('subtotal', v === '' ? null : Number(v)))(e.detail)} />
                </div>
                <div style={{ gridColumn: 'span 6' }}>
                  <TkInput mode="number" label="Discount Amount" value={String(form.discountAmount ?? '')} onTkChange={(e: CustomEvent) => ((v) => setField('discountAmount', v === '' ? null : Number(v)))(e.detail)} />
                </div>
                <div style={{ gridColumn: 'span 6' }}>
                  <TkInput mode="number" label="Vat Amount" value={String(form.vatAmount ?? '')} onTkChange={(e: CustomEvent) => ((v) => setField('vatAmount', v === '' ? null : Number(v)))(e.detail)} />
                </div>
                <div style={{ gridColumn: 'span 6' }}>
                  <TkInput mode="number" label="Grand Total" value={String(form.grandTotal ?? '')} onTkChange={(e: CustomEvent) => ((v) => setField('grandTotal', v === '' ? null : Number(v)))(e.detail)} />
                </div>
                <div style={{ gridColumn: 'span 6' }}>
                  <TkInput mode="text" label="Notes" value={String(form.notes ?? '')} onTkChange={(e: CustomEvent) => ((v) => setField('notes', v))(e.detail)} />
                </div>
                <div style={{ gridColumn: 'span 6' }}>
                  <LookupSelect label="Status *" value={String(form.status ?? '')} onChange={(v) => setField('status', v ? Number(v) : null)} searchable={false} options={[{ label: "Draft", value: "0" }, { label: "Confirmed", value: "1" }, { label: "Cancelled", value: "2" }]} />
                </div>
                <div style={{ gridColumn: 'span 6' }}>
                  <LookupSelect label="Firma *" resource="Company" value={String(form.companyId ?? '')} onChange={(v) => setField('companyId', v)} displayField="name" />
                </div>
            </div>
          </div>
          <div style={{ display: 'flex', justifyContent: 'flex-end', gap: 8, padding: '16px 28px', borderTop: '1px solid #e8e8e8', flexShrink: 0, background: '#fff' }}>
            <TkButton label="Cancel" variant="secondary" onTkClick={onClose} />
            <TkButton label={mutation.isPending ? 'Saving…' : 'Save Changes'} variant="primary" mode="submit" disabled={mutation.isPending} />
          </div>
        </form>
      </div>
    </div>
  );
};
