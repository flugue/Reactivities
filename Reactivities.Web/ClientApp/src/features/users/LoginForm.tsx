import * as React from 'react';
import { ErrorMessage, Form,Formik } from 'formik';
import TextInput from '../../app/common/form/TextInput';
import { Button, Header, Label } from 'semantic-ui-react';
import { observer } from 'mobx-react-lite';
import { useStore } from '../../app/stores/store';

function LoginForm() {
    const { userStore } = useStore();

    return (
        <Formik
            initialValues={{ email: '', password: '' }}
            onSubmit={(values, { setErrors }) => userStore.login(values).catch(error =>
                setErrors({ password: 'Invalid password', email: 'invalid email' }))}>
            {
                ({ handleSubmit, isSubmitting, errors }) => (
                    <Form className='ui form' onSubmit={handleSubmit} autoComplete='off'>
                        <Header as='h2' textAlign='center' content='Login to Reactivities' color='teal'/>
                        <TextInput name='email' placeholder='Email' />
                        <TextInput name='password' placeholder='Password' type='password' />
                        <ErrorMessage name="error" render={() => <Label style={{ marginBottom: 10 }} basic color='red' content={errors.email} />} />
                        <Button loading={isSubmitting} positive content='Login' type='submit' fluid />
                    </Form>
                )
            }
        </Formik>
        );
}

export default observer(LoginForm);