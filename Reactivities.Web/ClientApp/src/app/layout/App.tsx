import { observer } from 'mobx-react-lite';
import * as React from 'react';
import { useEffect } from 'react';
import { Route, Switch, useLocation } from 'react-router-dom';
import { ToastContainer } from 'react-toastify';
import { Container } from 'semantic-ui-react';
import ActivityDashboard from '../../features/activities/dashboard/ActivityDashboard';
import ActivityDetail from '../../features/activities/details/ActivityDetail';
import ActivityForm from '../../features/activities/form/ActivityForm';
import NotFound from '../../features/errors/NotFound';
import TestErrors from '../../features/errors/TestError';
import HomePage from '../../features/home/HomePage';
import LoginForm from '../../features/users/LoginForm';
import ModalContainer from '../common/modal/ModalContainer';
import { useStore } from '../stores/store';
import LoadingComponent from './LoadingComponent';
import NavBar from './NavBar';

function App() {
    const location = useLocation();
    const { commonStore, userStore } = useStore();

    useEffect(() => {
        if (commonStore.token) {
            userStore.getUser().finally(() => commonStore.setAppLoaded())
        } else {
            commonStore.setAppLoaded();
        }
    }, [commonStore, userStore]);

    if (!commonStore.appLoaded) return <LoadingComponent content='Loading App...' />
    
    return (
        <>
            <ToastContainer position='bottom-right' hideProgressBar />
            <ModalContainer/>
            <Route exact path="/" component={HomePage} />
            <Route
                path={'/(.+)'}
                render={() => (
                    <>
                        <NavBar />
                        <Container style={{ marginTop: '7em' }}>
                            <Switch>
                                <Route exact path="/activities" component={ActivityDashboard} />
                                <Route path="/activities/:id" component={ActivityDetail} />
                                <Route key={location.key} path={['/createActivity', '/manage/:id']} component={ActivityForm} />
                                <Route path='/login' component={LoginForm} />
                                <Route path='/errors' component={TestErrors} />
                                <Route component={NotFound} />
                            </Switch>
                        </Container>
                    </>
                )}

            />

        </>
    );
}

export default observer(App);